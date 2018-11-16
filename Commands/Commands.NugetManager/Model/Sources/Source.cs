using System;

namespace BeaverSoft.Texo.Commands.NugetManager.Model.Sources
{
    public class Source : ISource
    {
        public Source()
        {
            // no operation
        }

        public Source(Uri address)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }

        public Uri Address { get; set; }
    }
}
