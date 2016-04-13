using System;

namespace Client.Shared
{
    internal class ResolveConflictException<T> : Exception
    {
        private T clientItem;
        private T serverItem;

        public ResolveConflictException(T clientItem, T serverItem)
        {
            this.clientItem = clientItem;
            this.serverItem = serverItem;
        }
    }
}