namespace Redirect
{
    public class RedirectSettings
    {
        public string[] Urls { get; set; }
        public IUrlProcessor UrlProcessor { get; set; }
        public ICookieProcessor CookieProcessor { get; set; }
        public IContentProcessor ResponseContentProcessor { get; set; }
    }
}