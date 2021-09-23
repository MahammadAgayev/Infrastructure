using System;

namespace IdentityServer.Exceptions
{
    public class IdentityException : Exception
    {
        public IdentityException(string message) : base(message)
        {

        }
    }
}