var authCache = {}, groupsCache = {};

/**
 * Middleware for adding the email address and security groups to the
 * request.azureMobile.user object.
 * @param {express.Request} request the Express request
 * @param {express.Response} response the Express response
 * @param {function} next the next piece of middleware
 * @returns {any} the result of the next middleware
 */
function authMiddleware(request, response, next) {
  if (typeof request.azureMobile.user.id == 'undefined')
    return next();

  getEmailAddress(request.azureMobile.user)
  .then(function (emailaddress) {
    request.azureMobile.user.emailaddress = emailaddress;
    return getAADGroups(request.azureMobile.user);
  })
  .then(function (groups) {
    request.azureMobile.user.groups = groups;
    return next();
  });
}

/**
 * Obtain the email address of the user
 * @param {Azure.User} user the user request object
 * @returns {string} the email address
 */
function getEmailAddress(user) {
  // Check to see if the user is in the cache
  if (typeof authCache[user.id] !== 'undefined')
    return authCache[user.id];

  return user.getIdentity()
    .then(function (userInfo) {
      authCache[user.id] = userInfo.aad.claims.emailaddress;
      return authCache[user.id];
    });
}

/**
 * Obtain the groups the user belongs to
 * @param {Azure.User} user the user request object
 * @returns {string[]} the list of groups
 */
function getAADGroups(user) {
  if (typeof groupsCache[user.id] !== 'undefined')
    return groupsCache[user.id];

  return [];
}

module.exports = authMiddleware;
