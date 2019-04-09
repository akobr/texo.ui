using System;

namespace BeaverSoft.Texo.Core.View
{
    public interface ILink
    {
        string Title { get; }

        Uri Address { get; }
    }
}
