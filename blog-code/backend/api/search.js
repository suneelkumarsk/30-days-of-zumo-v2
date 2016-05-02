var express = require('express'),
    bodyParser = require('body-parser');

module.exports = function (configuration) {
    var router = express.Router();

    // Retrieve all records in the specified category
    router.get('/category/:category', function (req, res, next) {
        req.azureMobile.tables('items')
            .where({ category: req.params.category })
            .read()
            .then(results => res.json(results))
            .catch(next); // it is important to catch any errors and log them
    });

    return router;
};