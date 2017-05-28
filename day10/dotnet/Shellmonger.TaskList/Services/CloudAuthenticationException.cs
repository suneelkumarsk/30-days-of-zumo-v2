using System;

namespace Shellmonger.TaskList.Services
{
    internal class CloudAuthenticationException : Exception
    {
        public CloudAuthenticationException()
        {
        }

        public CloudAuthenticationException(string message) : base(message)
        {
        }

        public CloudAuthenticationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}