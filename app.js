var express = require('express'),
    serveStatic = require('serve-static'),
    azureMobileApps = require('azure-mobile-apps'),
    authMiddleware = require('./authMiddleware');

// Set up a standard Express app
var app = express();
var mobileApp = azureMobileApps({
    homePage: true,
    swagger: true
});

mobileApp.use(authMiddleware);
mobileApp.tables.import('./tables');
mobileApp.api.import('./api');

app.use(serveStatic('public'));

var npmRouter = express.Router();
npmRouter.use(serveStatic('node_modules', {
  dotfile: 'ignore', etag: true, index: false, lastModified: true
}));
app.use('/node_modules', npmRouter);

mobileApp.tables.initialize()
  .then(function () {
      app.use(mobileApp);
      app.listen(process.env.PORT || 3000);
  });
