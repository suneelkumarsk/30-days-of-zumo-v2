var azureMobileApps = require('azure-mobile-apps');

// Create a new table definition
var table = azureMobileApps.table();

// Require authentication
table.access = 'authenticated';

// CREATE operation
table.insert(function (context) {
    console.log('INSERT: context.user = ', context.user);
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
    console.log('UPDATE: context.user = ', context.user);
    context.query.where({ userId: context.user.emailaddress });
    context.item.userId = context.user.emailaddress;
    return context.execute();
});

// DELETE operation
table.delete(function (context) {
    console.log('DELETE: context.user = ', context.user);
    // Authorization - if Administrators is not in the group list, don't allow deletion
    if (context.user.groups.indexOf('92d92697-1242-4d38-9c1d-00f3ea0d0640') < 0) {
        console.log('user is not a member of Administrators');
        context.response.status(401).send('Only Administrators can do this');
        return;
    }

    console.log('user is a member of Administrators');
    context.query.where({ userId: context.user.emailaddress });
    return context.execute();
});

module.exports = table;
