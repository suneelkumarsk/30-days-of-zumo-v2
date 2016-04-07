
/**
 * Cache for the authentication information
 * @type {object}
 */
var authCache = {};

/**
 * Reducer method for converting groups into an array
 * @param {string[]} target the accumulator for the array
 * @param {object} clai the current claim being processed
 * @returns {string[]} the accumulator
 */
function groupReducer(target, claim) {
    if (claim.typ === 'groups')
        target.push(claim.val);
    return target;
}

/**
 * Middleware that adds the email address and security groups to
 * the request.azureMobile.user object
 * @param {express.Request} request the Express Request
 * @param {express.Response} response the Express Response
 * @param {function} next the next piece of middleware
 * @returns {any} the result of the next middleware
 */
function authMiddleware(request, response, next) {
    // Don't do anything if this is not an Azure Mobile Apps authenticated request
    if (typeof request.azureMobile === 'undefined') return next();
    if (typeof request.azureMobile.user === 'undefined') return next();
    if (typeof request.azureMobile.user.id === 'undefined') return next();

    // If data is in the cache, then use that information
    if (typeof authCache[request.azureMobile.user.id] !== 'undefined') {
        request.azureMobile.user.emailaddress = authCache[request.azureMobile.user.id].emailaddress;
        request.azureMobile.user.groups = authCache[request.azureMobile.user.id].groups;
        return next();
    }

    // Data is not in the cache - get the information and cache it
    request.azureMobile.user.getIdentity().then(function (userInfo) {
        var groups = userInfo.aad.user_claims.reduce(groupReducer, []);
        var email = userInfo.aad.claims.emailaddress || userInfo.aad.claims.upn;
        authCache[request.azureMobile.user.id] = {
            emailaddress: email,
            groups: groups
        };

        request.azureMobile.user.emailaddress = authCache[request.azureMobile.user.id].emailaddress;
        request.azureMobile.user.groups = authCache[request.azureMobile.user.id].groups;
        return next();
    });
}

module.xports = authMiddleware;
