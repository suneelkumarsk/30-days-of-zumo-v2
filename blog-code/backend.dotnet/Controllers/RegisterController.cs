using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System;

namespace backend.dotnet.Controllers
{
    [MobileAppController]
    public class RegisterController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post([FromBody] RegistrationViewModel model)
        {
            Console.WriteLine($"[RegisterController] ID = {model.pushChannel}");
            Console.WriteLine($"[RegisterController] Tags = {model.tags}");

            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }

    /// <summary>
    /// Format of the registration view model that is passed to the custom API
    /// </summary>
    public class RegistrationViewModel
    {
        public string pushChannel;

        public List<string> tags;
    }
}
