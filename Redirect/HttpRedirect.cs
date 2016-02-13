using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Redirect
{
    public sealed class HttpRedirect
    {
        private readonly HttpListener _listener;
        private readonly ICookieProcessor _cookieProcessor;
        private readonly IContentProcessor _responseContentProcessor;
        private readonly IUrlProcessor _urlProcessor;
        private bool _isWorking;

        public HttpRedirect(RedirectSettings settings)
        {
            _urlProcessor = settings.UrlProcessor;
            _cookieProcessor = settings.CookieProcessor ?? new CookieProcessor();
            _responseContentProcessor = settings.ResponseContentProcessor;

            _listener = new HttpListener();
            foreach (var url in settings.Urls)
            {
                _listener.Prefixes.Add(url);
            }
        }

        public event EventHandler<ProcessExceptionEventArgs> ProcessException;

        public void Start()
        {
            _isWorking = true;
            _listener.Start();
            Listen();
        }

        public void Stop()
        {
            _isWorking = false;
            _listener.Stop();
        }

        private void OnProcessException(Exception exception)
        {
            var handler = ProcessException;
            handler?.Invoke(this, new ProcessExceptionEventArgs(exception));
        }

        private async void Listen()
        {
            while (_isWorking)
            {
                try
                {
                    await ProcessRequestAsync(await _listener.GetContextAsync());
                }
                catch (Exception e)
                {
                    OnProcessException(e);
                }
            }
        }

        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            await Task.Yield();
            using (context.Response)
            {
                var utl = _urlProcessor.Process(context.Request.Url);
                var cookieContainer = _cookieProcessor.Process(utl, context.Request.Cookies);
                using (var handler = new HttpClientHandler {CookieContainer = cookieContainer})
                using (var client = new HttpClient(handler))
                {
                    var message = context.Request.ToMessage(utl);
                    var response = await client.SendAsync(message);
                    await context.Response.CopyFromAsync(response, _responseContentProcessor);
                    context.Response.Close();
                }
            }
        }
    }
}