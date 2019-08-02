using System;

namespace BeaverSoft.Texo.Core.View
{
    public class Link : ILink
    {
        public Link(string title, string address)
        {
            Title = title;
            Address = new Uri(address);
        }

        public Link(string title, Uri address)
        {
            Title = title;
            Address = address;
        }

        public string Title { get; }

        public Uri Address { get; }

        public static implicit operator Link((string title, string url) link)
        {
            return new Link(link.title, link.url);
        }
    }
}
