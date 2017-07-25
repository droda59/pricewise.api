using System;

namespace PriceAlerts.Common.Cleaners
{
    internal class BaseCleaner : ICleaner
    {
        private readonly Uri _baseUri;

        public BaseCleaner()
        {
        }

        protected BaseCleaner(Uri baseUri)
        {
            this._baseUri = baseUri;
        }

        public Uri Domain => this._baseUri;

        public virtual Uri CleanUrl(Uri originalUrl)
        {
            return originalUrl;
        }
    }
}
