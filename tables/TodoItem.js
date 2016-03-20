var azureMobileApps = require('azure-mobile-apps');

// Create a new table definition
var table = azureMobileApps.table();

table.read(function (context) {
    console.log('user = ', context.user);
    return context.execute();
});

module.exports = table;