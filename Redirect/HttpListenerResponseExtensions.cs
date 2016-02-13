using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Redirect
{
    internal static class HttpListenerResponseExtensions
    {
        private static readonly HashSet<string> Filter = new HashSet<string>();

        static HttpListenerResponseExtensions()
        {
            Filter.Add("Transfer-Encoding");
            Filter.Add("Content-Encoding");
            Filter.Add("Connection");
        }

        public static async Task CopyFromAsync(this HttpListenerResponse response, HttpResponseMessage message,
            IContentProcessor contentProcessor)
        {
            response.StatusCode = (int) message.StatusCode;
            foreach (var httpResponseHeader in message.Headers.Where(header => !Filter.Contains(header.Key)))
            {
                foreach (var value in httpResponseHeader.Value)
                {
                    response.AddHeader(httpResponseHeader.Key, value);
                }
            }
            foreach (var httpResponseHeader in message.Content.Headers.Where(header => !Filter.Contains(header.Key)))
            {
                foreach (var value in httpResponseHeader.Value)
                {
                    response.AddHeader(httpResponseHeader.Key, value);
                }
            }
            response.SendChunked = false;
            response.KeepAlive = false;

            var bytes = await message.Content.ReadAsByteArrayAsync();

            if (bytes.Length <= 0) return;

            if (contentProcessor != null)
            {
                bytes = contentProcessor.Process(bytes, message.Content.Headers.ContentType,
                    message.RequestMessage.RequestUri);
            }

            response.ContentLength64 = bytes.Length;
            await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}