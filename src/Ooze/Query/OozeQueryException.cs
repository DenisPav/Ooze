using System;

namespace Ooze.Query
{
    internal class OozeQueryException : Exception
    {
        public OozeQueryException(string message)
            : base(message)
        { }

        public OozeQueryException(
            string message,
            Exception innerException)
            : base(message, innerException)
        { }
    }
}
