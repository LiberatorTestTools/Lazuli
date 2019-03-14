using System;

namespace Liberator.Lazuli.Dokkit.Exceptions
{
    public class DokkitException : Exception
    {
        public DokkitException(string message, Exception exception) : base(message, exception)
        {

        }
    }
}
