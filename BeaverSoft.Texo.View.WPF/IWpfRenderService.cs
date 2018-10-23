using System.Windows.Documents;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.View.WPF
{
    public interface IWpfRenderService
    {
        Section Render(IItem item);
    }
}