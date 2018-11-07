using System;

namespace BeaverSoft.Texo.Commands.NugetManager.Model.Sources
{
    public class SourceInfo : ISourceInfo
    {
        public SourceInfo(Uri address)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }

        public Uri Address { get; }
    }
}
