using System.Collections.Generic;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Intellisense
{
    public interface IIntellisenseProvider
    {
        Task<IEnumerable<IItem>> GetHelpAsync(Input input);
    }
}
