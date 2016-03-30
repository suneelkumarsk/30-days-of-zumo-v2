var authCache = {};

/**
 * Reducer method for converting groups into an array
 * @param {string[]} target the accumulator for the array
 * @param {object} claim the current claim being processed
 * @returns {string[]} the accumulator
 */
function groupReducer(target, claim) {
    if (claim.typ === 'groups')
        target.push(claim.val);
    return target;
}
/**
 * Middleware for adding the email address and security groups to the
 * request.azureMobile.user object.
 * @param {express.Request} request the Express request
 * @param {express.Response} response the Express response
 * @param {function} next the next piece of middleware
 * @returns {any} the result of the next middleware
 */
function authMiddleware(request, response, next) {
    console.log('IN authMiddleware');
    if (typeof request.azureMobile.user === 'undefined') {
        console.log('request.azureMobile.user is not set');
        next();
        return;
    }
    if (typeof request.azureMobile.user.id === 'undefined') {
        console.log('request.azureMobile.user.id is not set');
        next();
        return;
    }

    if (typeof authCache[request.azureMobile.user.id] === 'undefined') {
        console.log('user', request.azureMobile.user.id, 'does not exist in authCache');
        request.azureMobile.user.getIdentity().then(function (userInfo) {
            console.log('back from getIdentity');
            console.log('emailaddress = ', userInfo.aad.claims.emailaddress);
            var groups = userInfo.aad.user_claims.reduce(groupReducer, []);
            console.log('groups = ', groups);
            authCache[request.azureMobile.user.id] = {
                emailaddress: userInfo.aad.claims.emailaddress,
                groups: groups
            };

            console.log('storing emailaddress in user object');
            request.azureMobile.user.emailaddress = authCache[request.azureMobile.user.id].emailaddress;
            console.log('storing groups in user object');
            request.azureMobile.user.groups = authCache[request.azureMobile.user.id].groups;
            console.log('calling next');
            next();
        });
    } else {
        console.log('user', request.azureMobile.user.id, 'exists in cache');
        request.azureMobile.user.emailaddress = authCache[request.azureMobile.user.id].emailaddress;
        request.azureMobile.user.groups = authCache[request.azureMobile.user.id].groups;
        console.log('calling next');
        next();
    }
}

module.exports = authMiddleware;
