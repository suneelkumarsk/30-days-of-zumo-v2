using Microsoft.Azure.Mobile.Server.Notifications;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace backend.dotnet.Controllers
{
    public class PushRegistrationController : ApiController
    {
        [HttpPut]
        [Route("push/installations/:id")]
        public HttpResponseMessage Register(string id, [FromBody] NotificationInstallation installation)
        {
            Debug.WriteLine($"PUSH: Install-ID   = {id}");
            Debug.WriteLine($"PUSH: Installation = {installation}");

            // Handle the response
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
