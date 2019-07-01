using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Actions
{
    public interface IAction
    {
        Task ExecuteAsync(IDictionary<string, string> arguments);
    }
}