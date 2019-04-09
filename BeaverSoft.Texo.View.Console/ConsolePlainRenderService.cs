using BeaverSoft.Texo.Core.View;
using SysConsole = System.Console;

namespace BeaverSoft.Texo.View.Console
{
    public class ConsolePlainRenderService : IConsoleRenderService
    {
        public void Write(IItem item)
        {
            SysConsole.WriteLine(item.Text);
        }
    }
}
