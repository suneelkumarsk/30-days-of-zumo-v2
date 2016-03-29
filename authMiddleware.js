var authCache = {};

function authMiddleware(request, response, next) {
  if (typeof request.azureMobile.user.id !== 'undefined') {

    // Check to see if the user is in the cache
    if (typeof authCache[request.azureMobile.user.id] !== 'undefined') {
      request.azureMobile.user.emailaddress = authCache[request.azureMobile.user.id];
      return next();
    }

    // If not in the cache then call getIdentity() to get the information
    request.azureMobile.user.getIdentity().then(function (userInfo) {
      console.log('USER INFO CLAIMS: ', JSON.stringify(userInfo.aad.claims, 2));
      if (typeof userInfo.aad.claims.emailaddress !== 'undefined') {
        authCache[request.azureMobile.user.id] = userInfo.aad.claims.emailaddress;
        request.azureMobile.user.emailaddress = authCache[request.azureMobile.user.id];
      }
      return next();
    });

  } else {
    return next();
  }
}

module.exports = authMiddleware;
