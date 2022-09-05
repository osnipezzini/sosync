using System.Runtime.Serialization;

namespace SOSync.Common.Exceptions
{
    [Serializable]
    public class APIServiceException : Exception
    {
        public APIServiceException() { }
        public APIServiceException(string message) : base(message) { }
        public APIServiceException (string message, Exception innerException) : base(message, innerException) { }
        protected APIServiceException(
            SerializationInfo info,
            StreamingContext context ) : base(info, context ) { }
    }
}
