-- The general plan is that my existing application has a set of tables.
-- These are stored in SCHEMA [myapp].  I'm going to place my mobile
-- projections of these tables in [mobile].
CREATE SCHEMA myapp;
GO

CREATE SCHEMA mobile;
GO

-- Create the tables for my enterprise application.  There is nothing special
-- about these tables - just make them like you normally would.  However, note
-- that relationships between tables are problematic - each update from the
-- mobile side is done individually.

CREATE TABLE [myapp].[TodoItem] (
    [id]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserId]   NVARCHAR (255) NOT NULL,
    [Title]    NVARCHAR (255) NOT NULL,
    [Complete] BIT            NULL
);
GO

-- MOBILE PROJECTIONS
-- For each table I want to project into the mobile space, there is a System
-- Properties table and a VIEW.  The view is the combination of the enterprise
-- application table and the system properties table.

CREATE TABLE [mobile].[TodoItem_SystemProps] (
    [id]        NVARCHAR (255)     CONSTRAINT [DF_todoitem_id] DEFAULT (CONVERT([nvarchar](255),newid(),(0))) NOT NULL,
    [createdAt] DATETIMEOFFSET (7) CONSTRAINT [DF_todoitem_createdAt] DEFAULT (CONVERT([datetimeoffset](7),sysutcdatetime(),(0))) NOT NULL,
    [updatedAt] DATETIMEOFFSET (7) NULL,
    [version]   ROWVERSION         NOT NULL,
    [deleted]   BIT                DEFAULT ((0)) NOT NULL,
    [item_id]   BIGINT             NOT NULL,    -- This matches the [id] field in your enterprise app table
    PRIMARY KEY NONCLUSTERED ([id] ASC)
);
GO

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
    [myapp].[TodoItem].[id] = [mobile].[TodoItem_SystemProps].[item_id];
GO

-- In addition to the system properties table and the view, you need to implement
-- six triggers.  Three (Insert, Update, Delete) handle the case when data is
-- inserted into the enterprise application table, and three (again, Insert, Update
-- and Delete) handle the data from Azure Mobile Apps.

-- Triggers for handling modifications to the enterprise app table
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
END;
GO

CREATE TRIGGER
    [myapp].[TRG_TodoItem_Insert]
ON
    [myapp].[TodoItem]
AFTER
    INSERT
AS BEGIN
    DECLARE @itemid AS BIGINT
    SELECT @itemid = inserted.id FROM inserted
    INSERT INTO
        [mobile].[TodoItem_SystemProps] ([item_id], [updatedAt])
    VALUES
        (@itemid, CONVERT(DATETIMEOFFSET(7), SYSUTCDATETIME()));
END;
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
END;
GO

-- Triggers for handlin modifications done via Azure Mobile Apps
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
END;
GO

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

    IF UPDATE(Id) BEGIN
        DECLARE @itemid AS BIGINT
        SELECT @itemid = @@identity
        DECLARE @id AS NVARCHAR(255)
        SELECT @id = inserted.Id FROM inserted
        UPDATE [mobile].[TodoItem_SystemProps] SET [Id] = @id WHERE [item_id] = @itemid
    END
END;
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
END;
GO
