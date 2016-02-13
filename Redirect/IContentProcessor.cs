using System;
using System.Net.Http.Headers;

namespace Redirect
{
    public interface IContentProcessor
    {
        byte[] Process(byte[] bytes, MediaTypeHeaderValue type, Uri url);
    }
}