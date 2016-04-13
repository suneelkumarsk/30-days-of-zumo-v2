using System;

namespace Client.UWP.Services
{
    internal class CloudAuthenticationFailedException : Exception
    {
        public CloudAuthenticationFailedException()
        {
        }

        public CloudAuthenticationFailedException(string message) : base(message)
        {
        }

        public CloudAuthenticationFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}