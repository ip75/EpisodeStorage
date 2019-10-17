using System;

namespace Detector.Common
{
    public static class DetectorException
    {
        public static Exception CreateError(string code, string message = null, Exception innerException = null, object properties = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                message = string.Empty;

            var exception = innerException != null ? new ErrorException(message, innerException) : new ErrorException(message);

            exception.Code = code;
            exception.Properties = properties;

            return exception;
        }
    }
}
