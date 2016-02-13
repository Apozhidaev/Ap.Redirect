using System;

namespace Redirect
{
    public interface IUrlProcessor
    {
        Uri Process(Uri url);
    }
}