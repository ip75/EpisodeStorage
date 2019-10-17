using System;

namespace Detector.Common
{
    public class ErrorException : Exception
    {
        public ErrorException(string message) : base(message)
        {
        }
        public ErrorException(string message, Exception innerException) : base (message, innerException)
        {
        }
        public string Code { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        internal object Properties { get; set; }
    }
}
