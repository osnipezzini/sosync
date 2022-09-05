using System.Runtime.Serialization;

namespace SOSync.Common.Exceptions
{
    [Serializable]
    public class NotInicializedException : Exception
    {
        public NotInicializedException() { }
        public NotInicializedException(string message) : base(message) { }
        public NotInicializedException(string message, Exception innerException) : base(message, innerException) { }
        protected NotInicializedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }

    }
}
