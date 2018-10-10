using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public interface IQuery : IInputStatement
    {
        IImmutableList<IQuery> Queries { get; }

        IImmutableList<IOption> Options { get; }

        string DefaultQueryKey { get; }
    }
}