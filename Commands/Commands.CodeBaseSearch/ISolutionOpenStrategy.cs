using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Commands.CodeBaseSearch
{
    public interface ISolutionOpenStrategy
    {
        Task<Workspace> OpenAsync();
    }
}
