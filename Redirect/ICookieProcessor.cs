using System;
using System.Net;

namespace Redirect
{
    public interface ICookieProcessor
    {
        CookieContainer Process(Uri url, CookieCollection cookieCollection);
    }
}