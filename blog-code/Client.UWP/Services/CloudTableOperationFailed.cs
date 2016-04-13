using System;

namespace Client.UWP.Services
{
    internal class CloudTableOperationFailed : Exception
    {
        public CloudTableOperationFailed()
        {
        }

        public CloudTableOperationFailed(string message) : base(message)
        {
        }

        public CloudTableOperationFailed(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}