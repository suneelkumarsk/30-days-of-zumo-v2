var azureMobileApps = require('azure-mobile-apps');

// Create a new table definition
var table = azureMobileApps.table();

// Require authentication
table.access = 'anonymous';

// CREATE operation
table.insert(function (context) {
    context.item.userId = context.user.emailaddress;
    return context.execute();
});

// READ operation
table.read(function (context) {
    console.log('READ: context.user = ', context.user);
    context.query.where({ userId: context.user.emailaddress });
    return context.execute();
});

// UPDATE operation
table.update(function (context) {
    context.query.where({ userId: context.user.emailaddress });
    context.item.userId = context.user.emailaddress;
    return context.execute();
});

// DELETE operation
table.delete(function (context) {
    context.query.where({ userId: context.user.emailaddress });
    return context.execute();
});

module.exports = table;
