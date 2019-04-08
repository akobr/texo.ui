using System;

namespace BeaverSoft.Texo.Core.View
{
    public interface IActionLink
    {
        string Name { get; }

        string Title { get; }

        Uri Address { get; }
    }
}