using System;

namespace Client.Shared
{
    internal class AzureSyncException : Exception
    {
        public AzureSyncException()
        {
        }

        public AzureSyncException(string message) : base(message)
        {
        }

        public AzureSyncException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}