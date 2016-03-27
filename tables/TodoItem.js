var azureMobileApps = require('azure-mobile-apps');

// Create a new table definition
var table = azureMobileApps.table();

// Require authentication
table.access = 'disabled';

// CREATE operation
table.insert(function (context) {
  return context.user.getIdentity().then(function (userInfo) {
    context.item.userId = userInfo.aad.claims.emailaddress;
    return context.execute();
  });
});

// READ operation
table.read(function (context) {
  console.log('context.user = ', JSON.stringify(context.user));
  return context.user.getIdentity().then(function (userInfo) {
    console.log('userInfo.aad.claims = ', JSON.stringify(userInfo.aad.claims));
    context.query.where({ userId: userInfo.aad.claims.emailaddress });
    return context.execute();
  });
});

// UPDATE operation
table.update(function (context) {
  return context.user.getIdentity().then(function (userInfo) {
    context.query.where({ userId: userInfo.aad.claims.emailaddress });
    context.item.userId = userInfo.aad.claims.emailaddress;
    return context.execute();
  });
});

// DELETE operation
table.delete(function (context) {
  return context.user.getIdentity().then(function (userInfo) {
    context.query.where({ userId: userInfo.aad.claims.emailaddress });
    return context.execute();
  });
});

module.exports = table;
