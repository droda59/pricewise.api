using System;

namespace PriceAlerts.Common
{
    internal class ParseException : Exception
    {
        public ParseException(string message, Exception innerException, Uri uri)
            : base(message, innerException)
        {
            this.Uri = uri;
        }

        public ParseException(string message, Uri uri)
            : base(message)
        {
            this.Uri = uri;
        }

        public Uri Uri { get; private set; }

        public override string ToString()
        {
            return $"{this.Message} on {this.Uri.AbsoluteUri}";
        }
    }
}
