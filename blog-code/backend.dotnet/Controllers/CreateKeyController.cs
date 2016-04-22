using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using System.Web;
using System.Net;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using System.Collections.Generic;
using Jose;

namespace backend.dotnet.Controllers
{
    [MobileAppController]
    public class CreateKeyController : ApiController
    {
        // GET api/CreateKey
        public Dictionary<string, string> Get()
        {
            var now = DateTime.UtcNow.ToString("yyyy-M-d");
            Debug.WriteLine($"NOW = {now}");
            var installID = HttpContext.Current.Request.Headers["X-INSTALLATION-ID"];
            if (installID == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            Debug.WriteLine($"INSTALLID = {installID}");

            var subject = $"{now}-{installID}";
            var token = createMD5(subject);
            var issuer = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME");
            if (issuer == null)
            {
                issuer = "unk";
            }
            Debug.WriteLine($"SUBJECT = {subject}");
            Debug.WriteLine($"TOKEN = {token}");

            var expires = ((TimeSpan)(DateTime.UtcNow.AddHours(4) - new DateTime(1970, 1, 1))).TotalMilliseconds;
            var payload = new Dictionary<string, object>()
            {
                { "aud", installID },
                { "iss", issuer },
                { "sub", subject },
                { "exp", expires },
                { "token", token }
            };

            byte[] secretKey = Encoding.ASCII.GetBytes(installID);
            var result = new Dictionary<string, string>()
            {
                { "jwt", JWT.Encode(payload, secretKey, JwsAlgorithm.HS256) }
            };

            return result;
        }

        /// <summary>
        /// Compute an MD5 hash of a string
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>The MD5 hash as a string of hex</returns>
        private string createMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] ib = Encoding.ASCII.GetBytes(input);
                byte[] ob = md5.ComputeHash(ib);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < ob.Length; i++)
                {
                    sb.Append(ob[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
