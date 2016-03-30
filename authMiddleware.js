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
    if (typeof request.azureMobile.user === 'undefined') {
        return next();
    if (typeof request.azureMobile.user.id === 'undefined') {
        return next();

    if (typeof authCache[request.azureMobile.user.id] === 'undefined') {
        request.azureMobile.user.getIdentity().then(function (userInfo) {
            var groups = userInfo.aad.user_claims.reduce(groupReducer, []);
            authCache[request.azureMobile.user.id] = {
                emailaddress: userInfo.aad.claims.emailaddress,
                groups: groups
            };

            request.azureMobile.user.emailaddress = authCache[request.azureMobile.user.id].emailaddress;
            request.azureMobile.user.groups = authCache[request.azureMobile.user.id].groups;
            next();
        });
    } else {
        request.azureMobile.user.emailaddress = authCache[request.azureMobile.user.id].emailaddress;
        request.azureMobile.user.groups = authCache[request.azureMobile.user.id].groups;
        next();
    }
}

module.exports = authMiddleware;
