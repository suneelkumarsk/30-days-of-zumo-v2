var express = require('express');
var jwt = require('jsonwebtoken');
var md5 = require('md5');

var router = express.Router();

router.get('/createKey', function (req, res, next) {
    var d = new Date();
    var now = d.getUTCFullYear() + '-' + (d.getUTCMonth() + 1) + '-' + d.getUTCDate();
    console.info('NOW = ', now);
    var installID = req.get('X-ZUMO-INSTALLATION-ID');
    console.info('INSTALLID = ', installID);

    if (typeof installID === 'undefined') {
        console.info('NO INSTALLID FOUND');
        res.status(400).send({ error: "Invalid Installation ID" });
        return;
    }

    var subject = now + installID;
    var token = md5(subject);
    console.info('TOKEN = ', token);

    var payload = {
        token: token
    };

    var options = {
        expiresIn: '4h',
        audience: installID,
        issuer: process.env.WEBSITE_SITE_NAME || 'unk',
        subject: subject
    };

    var signedJwt = jwt.sign(payload, installID, options);
    res.status(200).send({ jwt: signedJwt });
});

module.exports = router;
