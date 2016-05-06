var express = require('express'),
    bodyParser = require('body-parser'),
    notifications = require('azure-mobile-apps/src/notifications'),
    log = require('azure-mobile-apps/src/log');

module.exports = function (configuration) {
    var router = express.Router(),
        installationClient;

    if (configuration && configuration.notifications && Object.keys(configuration.notifications).length > 0) {
        router.use(addPushContext);
        router.route('/:installationId')
            .put(bodyParser.json(), put, errorHandler)
            .delete(del, errorHandler);

        installationClient = notifications(configuration.notifications);
    }

    return router;

    function addPushContext(req, res, next) {
        req.azureMobile = req.azureMobile || {};
        req.azureMobile.push = installationClient.getClient();
        next();
    }

    function put(req, res, next) {
        var installationId = req.params.installationId,
            installation = req.body,
            tags = [],
            user = req.azureMobile.user;

        // White list of all known tags
        var whitelist = [
            'news',
            'sports'
        ];

        // Logic for determining the correct list of tags
        installations.tags.forEach(function (tag) {
            if (whitelist.indexOf(tag.toLowerCase()) !== -1)
                tags.push(tag.toLowerCase());
        });
        // Add in the "automatic" tags
        if (user) {
            tags.push('$userid:' + user.id);
            if (user.emailaddress) tags.push('$email:' + user.emailaddress);
        }
        // Replace the installation tags requested with my list
        installation.tags = tags;

        installationClient.putInstallation(installationId, installation, user && user.id)
            .then(function (result) {
            res.status(204).end();
        })
            .catch(next);
    }

    function del(req, res, next) {
        var installationId = req.params.installationId;

        installationClient.deleteInstallation(installationId)
            .then(function (result) {
            res.status(204).end();
        })
            .catch(next);
    }

    function errorHandler(err, req, res, next) {
        log.error(err);
        res.status(400).send(err.message || 'Bad Request');
    }
};
