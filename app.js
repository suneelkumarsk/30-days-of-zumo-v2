var express = require('express'),
    azureMobileApps = require('azure-mobile-apps');

// Set up a standard Express app
var app = express();
var mobileApp = azureMobileApps({
    homePage: true,
    swagger: true
});

mobileApp.tables.import('./tables');
mobileApp.api.import('./api');

mobileApp.tables.initialize()
  .then(function () {
      app.use(mobileApp);
      app.listen(process.env.PORT || 3000);
  });
