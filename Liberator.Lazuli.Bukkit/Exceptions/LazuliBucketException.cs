using NUnit.Framework;
using System;
using System.Diagnostics;

namespace Liberator.Lazuli.Minio.Exceptions
{
    /// <summary>
    /// Exception for the Lazuli library
    /// </summary>
    public class LazuliBucketException : Exception
    {
        /// <summary>
        /// Exception thrown by the Lazuli library.
        /// </summary>
        /// <param name="message">The message to send with the exception</param>
        /// <param name="e">The inner exception.</param>
        public LazuliBucketException(string message, Exception e) : base(message, e)
        {
            string callingFunction = new StackTrace().GetFrame(1).GetMethod().Name;
            Assert.Fail();
        }
    }
}
