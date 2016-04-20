-- Database Schema Creation for the split-table view
CREATE SCHEMA [mobile]
GO

CREATE SCHEMA [myapp]
GO

-- The original table - the idea is that this is just a normal table, like you would find in
-- any NORMAL MVC / Web App for example - it doesn't have any of the special characteristics
-- of an Azure Mobile Apps table
CREATE TABLE [myapp].[TodoItem] (
  [id]       BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  [UserId]   NVARCHAR(255) NOT NULL,
  [Title]     NVARCHAR(255) NOT NULL,
  [Complete] BIT
)
GO

-- The System Properties table that complements the original table to make it Azure Mobile Apps
-- compatible.  This is joined with the [item_id] that really should be a FOREIGN KEY, but we
-- can do without it.  I larger systems, you probably want to index updatedAt, id, deleted and
-- item_id
CREATE TABLE [mobile].[TodoItem_SystemProps] (
  [id]        NVARCHAR(255) CONSTRAINT [DF_todoitem_id] DEFAULT (CONVERT([NVARCHAR](255),NEWID(),(0))) NOT NULL,
  [createdAt] DATETIMEOFFSET(7) CONSTRAINT [DF_todoitem_createdAt] DEFAULT (CONVERT(DATETIMEOFFSET(3),SYSUTCDATETIME(),(0))) NOT NULL,
  [updatedAt] DATETIMEOFFSET(7) NULL,
  [version]   ROWVERSION NOT NULL,
  [deleted]   BIT DEFAULT ((0)) NOT NULL,
  [item_id]   BIGINT NOT NULL
  PRIMARY KEY NONCLUSTERED ([id] ASC)
)
GO

-- We use a view to combine the two tables together to produce a composite table
CREATE VIEW [mobile].[TodoItem] AS
SELECT
    [mobile].[TodoItem_SystemProps].[id],
    [mobile].[TodoItem_SystemProps].[createdAt],
    [mobile].[TodoItem_SystemProps].[updatedAt],
    [mobile].[TodoItem_SystemProps].[version],
    [mobile].[TodoItem_SystemProps].[deleted],
    [mobile].[TodoItem_SystemProps].[item_id],
    [myapp].[TodoItem].[UserId],
    [myapp].[TodoItem].[Title],
    [myapp].[TodoItem].[Complete]
FROM
    [myapp].[TodoItem],
    [mobile].[TodoItem_SystemProps]
WHERE
    [myapp].[TodoItem].[id] = [mobile].[TodoItem_SystemProps].[item_id]
GO

-- The first set of three triggers are used to update the system properties when the main
-- table is updated.
CREATE TRIGGER
    [myapp].[TRG_TodoItem_Insert]
ON
    [myapp].[TodoItem]
AFTER
    INSERT
AS BEGIN
    DECLARE @itemid AS BIGINT
    SELECT @itemid = inserted.id FROM inserted
    INSERT INTO [mobile].[TodoItem_SystemProps] ([item_id], [updatedAt]) VALUES (@itemid, CONVERT(DATETIMEOFFSET(7), SYSUTCDATETIME()));
END
GO

CREATE TRIGGER
    [myapp].[TRG_TodoItem_Update]
ON
    [myapp].[TodoItem]
AFTER
    UPDATE
AS BEGIN
    UPDATE
        [mobile].[TodoItem_SystemProps]
    SET
        [updatedAt] = CONVERT(DATETIMEOFFSET(7), SYSUTCDATETIME())
    FROM
        INSERTED
    WHERE
        INSERTED.id = [mobile].[TodoItem_SystemProps].[item_id]
END
GO

CREATE TRIGGER
    [myapp].[TRG_TodoItem_Delete]
ON
    [myapp].[TodoItem]
AFTER
    DELETE
AS BEGIN
    DECLARE @itemid AS BIGINT
    SELECT @itemid = deleted.id from deleted
    DELETE FROM [mobile].[TodoItem_SystemProps] WHERE [item_id] = @itemid
END
GO

-- The second set of triggers are used when the VIEW is updated via Azure Mobile Apps
CREATE TRIGGER
    [mobile].[TRG_Mobile_TodoItem_Insert]
ON
    [mobile].[TodoItem]
INSTEAD OF
    INSERT
AS BEGIN
    DECLARE @userid AS NVARCHAR(255)
    SELECT @userid = inserted.UserId FROM inserted
    DECLARE @title AS NVARCHAR(255)
    SELECT @title = inserted.Title FROM inserted
    DECLARE @complete AS BIT
    SELECT @complete = inserted.Complete FROM inserted

    INSERT INTO
        [myapp].[TodoItem] ([UserId], [Title], [Complete])
    VALUES
        (@userid, @title, @complete)
END
GO

CREATE TRIGGER
    [mobile].[TRG_Mobile_TodoItem_Update]
ON
    [mobile].[TodoItem]
INSTEAD OF
    UPDATE
AS BEGIN
    DECLARE @id AS NVARCHAR(255)
    SELECT @id = inserted.id FROM inserted
    DECLARE @itemid AS BIGINT
    SELECT @itemid = [item_id] FROM [mobile].[TodoItem_SystemProps] WHERE [id] = @id

    IF UPDATE(UserId) BEGIN
        DECLARE @userid AS NVARCHAR(255)
        SELECT @userid = inserted.UserId FROM inserted
        UPDATE [myapp].[TodoItem] SET [UserId] = @userid WHERE [id] = @itemid
    END
    IF UPDATE(Title) BEGIN
        DECLARE @title AS NVARCHAR(255)
        SELECT @title = inserted.Title FROM inserted
        UPDATE [myapp].[TodoItem] SET [Title] = @title WHERE [id] = @itemid
    END
    IF UPDATE(Complete) BEGIN
        DECLARE @complete AS BIT
        SELECT @complete = inserted.Complete FROM inserted
        UPDATE [myapp].[TodoItem] SET [Complete] = @complete WHERE [id] = @itemid
    END
    IF UPDATE(deleted) BEGIN
        DECLARE @deleted AS BIT
        SELECT @deleted = inserted.deleted FROM inserted
        UPDATE [mobile].[TodoItem_SystemProps] SET [deleted] = @deleted WHERE [item_id] = @itemid
    END
END
GO

CREATE TRIGGER
    [mobile].[TRG_Mobile_TodoItem_Delete]
ON
    [mobile].[TodoItem]
INSTEAD OF
    DELETE
AS BEGIN
    DECLARE @id AS NVARCHAR(255)
    SELECT @id = deleted.id FROM deleted
    DECLARE @itemid AS BIGINT
    SELECT @itemid = [item_id] FROM [mobile].[TodoItem_SystemProps] WHERE [id] = @id

    DELETE FROM [myapp].[TodoItem] WHERE [id] = @itemid
    DELETE FROM [mobile].[TodoItem_SystemProps] WHERE [id] = @id
END
GO
