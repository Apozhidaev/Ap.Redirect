using System;
using System.Net;

namespace Redirect
{
    public class CookieProcessor : ICookieProcessor
    {
        public CookieContainer Process(Uri url, CookieCollection cookieCollection)
        {
            var cookieContainer = new CookieContainer();
            var uriBuilder = new UriBuilder
            {
                Scheme = url.Scheme,
                Host = url.Host,
                Port = url.Port
            };
            cookieContainer.Add(uriBuilder.Uri, cookieCollection);
            return cookieContainer;
        }
    }
}