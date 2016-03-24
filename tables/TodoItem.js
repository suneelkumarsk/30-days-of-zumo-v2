var azureMobileApps = require('azure-mobile-apps');

// Create a new table definition
var table = azureMobileApps.table();

// Require authentication
table.access = 'authenticated';

// CREATE operation
table.insert(function (context) {
  context.item.userId = context.user.id;
  return context.execute();
});

// READ operation
table.read(function (context) {
  context.user.getIdentity().then(function (userInfo) {
    console.log('user.getIdentity = ', JSON.stringify(userInfo));
    context.query.where({ userId: context.user.id });
    return context.execute();
  });
});

// UPDATE operation
table.update(function (context) {
  context.query.where({ userId: context.user.id });
  context.item.userId = context.user.id;
  return context.execute();
});

// DELETE operation
table.delete(function (context) {
  context.query.where({ userId: context.user.id });
  return context.execute();
});

module.exports = table;
