var express = require('express'),
    serveStatic = require('serve-static'),
    azureMobileApps = require('azure-mobile-apps'),
    authMiddleware = require('./authMiddleware');

// Set up a standard Express app
var webApp = express();

// Set up the Azure Mobile Apps SDK
var mobileApp = azureMobileApps();
mobileApp.use(authMiddleware);
mobileApp.tables.import('./tables');

// Create the public app area
webApp.use(serveStatic('public'));

// Initialize the Azure Mobile Apps, then start listening
mobileApp.tables.initialize().then(function () {
    webApp.use(mobileApp);
    webApp.listen(process.env.PORT || 3000);
});

