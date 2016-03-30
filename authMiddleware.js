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
        return next();
    }
    if (typeof request.azureMobile.user.id === 'undefined') {
        console.log('request.azureMobile.user.id is not set');
        return next();
    }

    var user = request.azureMobile.user;
    if (typeof authCache[user.id] === 'undefined') {
        console.log('user', user.id, 'does not exist in authCache');
        return user.getIdentity().then(function (userInfo) {
            console.log('back from getIdentity: emailaddress = ', userInfo.claims.aad.emailaddress);
            var groups = userInfo.aad.user_claims.reduce(groupReducer, []);
            console.log('groups = ', groups);
            authCache[user.id] = {
                emailaddress: userInfo.claims.aad.emailaddress,
                groups: groups
            };

            console.log('storing emailaddress in user object');
            request.azureMobile.user.emailaddress = authCache[user.id].emailaddress;
            console.log('storing groups in user object');
            request.azureMobile.user.groups = authCache[user.id].groups;
            console.log('calling next');
            return next();
        });
    } else {
        console.log('user', user.id, 'exists in cache');
        request.azureMobile.user.emailaddress = authCache[user.id].emailaddress;
        request.azureMobile.user.groups = authCache[user.id].groups;
        console.log('calling next');
        return next();
    }
}

module.exports = authMiddleware;
