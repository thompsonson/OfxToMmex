using System;
using System.Runtime.Serialization;
using log4net;

namespace OfxToMmexConsoleApp
{
    [Serializable]
    class OfxToMmexException : Exception
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OfxToMmexException));

        // TODO: Email out the exception
        public OfxToMmexException()
        {
        }

        public OfxToMmexException(string message)
            : base(message)
        {
        }

        public OfxToMmexException(string message, Exception inner)
            : base(message, inner)
        {
            log.Fatal(inner.ToString());
        }

        protected OfxToMmexException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
