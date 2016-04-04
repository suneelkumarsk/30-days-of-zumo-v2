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
    return context.tables('friend')
        .where({ viewer: context.user.emailaddress })
        .select('userId')
        .read()
        .then(function (friends) {
            var list = friends.map(function (f) { return f.userId; }).push(context.user.emailaddress);
            console.log('READ: friends = ', list);
            context.query.where('function(friends) { return this.userId in friends; }', list);
            return context.execute();
        });
//    context.query.where({ userId: context.user.emailaddress });
//    return context.execute();
});

// UPDATE operation
table.update(function (context) {
    console.log('UPDATE: context.user = ', context.user);
    context.query.where({ userId: context.user.emailaddress });
    context.item.userId = context.user.emailaddress;
    return context.execute();
});

// DELETE operation
function isAdministrator(request, response, next) {
    if (request.azureMobile.user.groups.indexOf('92d92697-1242-4d38-9c1d-00f3ea0d0640') < 0) {
        response.status(401).send('Only Administrators can do this');
        return;
    }
    next();
}

table.delete.use(isAdministrator, table.operation);
table.delete(function (context) {
    context.query.where({ userId: context.user.emailaddress });
    return context.execute();
});

module.exports = table;
