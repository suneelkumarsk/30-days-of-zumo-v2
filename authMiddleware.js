var authCache = {};

function authMiddleware(request, response, next) {
  if (typeof request.azureMobile.user.id !== 'undefined') {
    if (typeof authCache[request.azureMobile.user.id] !== 'undefined') {
      request.azureMobile.user.emailaddress = authCache[request.azureMobile.user.id];
      next();
    }
    request.azureMobile.user.getIdentity().then(function (userInfo) {
      if (typeof userInfo.aad.claims.emailaddress !== 'undefined') {
        authCache[request.azureMobile.user.id] = userInfo.aad.claims.emailaddress;
        request.azureMobile.user.emailaddress = authCache[request.azureMobile.user.id];
      }
      next();
    });
  } else {
    next();
  }
}

module.exports = authMiddleware;
