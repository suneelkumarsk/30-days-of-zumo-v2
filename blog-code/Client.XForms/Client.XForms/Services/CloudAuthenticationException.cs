using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.XForms.Services
{
    /// <summary>
    /// Standard exception that will be thrown if authentication fails
    /// </summary>
    public class CloudAuthenticationException : Exception
    {
        public CloudAuthenticationException() : base() { }
        public CloudAuthenticationException(string message) : base(message) { }
        public CloudAuthenticationException(string message, Exception inner):base(message,inner) { }
    }
}
