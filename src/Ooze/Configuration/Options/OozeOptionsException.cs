using System;

namespace Ooze.Configuration.Options
{
    internal class OozeOptionsException : Exception
    {
        public OozeOptionsException(string message)
            : base(message)
        { }

        public OozeOptionsException(
            string message,
            Exception innerException)
            : base(message, innerException)
        { }
    }
}
