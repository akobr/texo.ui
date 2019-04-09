using System;

namespace BeaverSoft.Texo.Core.View
{
    public interface IAction
    {
        string Name { get; }

        string Title { get; }

        Uri Address { get; }
    }
}