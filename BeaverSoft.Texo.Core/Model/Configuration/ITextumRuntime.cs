using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public interface ITextumRuntime
    {
        IImmutableList<IQuery> Commands { get; }

        string DefaultCommand { get; }
    }
}