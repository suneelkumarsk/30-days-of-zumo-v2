var express = require('express'),
    serveStatic = require('serve-static'),
    azureMobileApps = require('azure-mobile-apps'),
    authMiddleware = require('./authMiddleware'),
    customRouter = require('./customRouter'),
    pushRegistrationHandler = require('./pushRegistration');

// Set up a standard Express app
var webApp = express();

// Set up the Azure Mobile Apps SDK
var mobileApp = azureMobileApps({
    notificationRootPath: '/.push/disabled'
});

mobileApp.use(authMiddleware);
mobileApp.tables.import('./tables');
mobileApp.api.import('./api');
mobileApp.use('/push/installations', pushRegistrationHandler);

// Create the public app area
webApp.use(serveStatic('public'));

// Initialize the Azure Mobile Apps, then start listening
mobileApp.tables.initialize().then(function () {
    webApp.use(mobileApp);
    webApp.use('/custom', customRouter);

    webApp.listen(process.env.PORT || 3000);
});

