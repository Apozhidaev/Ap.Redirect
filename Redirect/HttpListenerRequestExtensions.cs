using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Redirect
{
    internal static class HttpListenerRequestExtensions
    {
        private static readonly HashSet<string> Filter = new HashSet<string>();

        static HttpListenerRequestExtensions()
        {
            Filter.Add("Accept-Encoding");
            Filter.Add("Content-Length");
            Filter.Add("Content-Type");
            Filter.Add("Connection");
        }

        public static HttpRequestMessage ToMessage(this HttpListenerRequest request, Uri url)
        {
            var message = new HttpRequestMessage();

            foreach (var headerName in request.Headers.AllKeys.Where(header => !Filter.Contains(header)))
            {
                var headerValues = request.Headers.GetValues(headerName);
                if (!message.Headers.TryAddWithoutValidation(headerName, headerValues))
                {
                    message.Content.Headers.TryAddWithoutValidation(headerName, headerValues);
                }
            }
            message.Method = new HttpMethod(request.HttpMethod);
            message.RequestUri = url;
            if (request.ContentLength64 > 0)
            {
                message.Content = new StreamContent(request.InputStream);
            }
            message.Headers.Host = url.Host;
            if (!string.IsNullOrEmpty(request.ContentType))
            {
                message.Content.Headers.ContentType = new MediaTypeHeaderValue(request.ContentType);
            }
            message.Headers.ConnectionClose = true;
            message.Headers.AcceptEncoding.Clear();
            return message;
        }
    }
}