using BeaverSoft.Texo.Core.Model.View;

using SysConsole = System.Console;

namespace BeaverSoft.Texo.View.Console
{
    public class ConsoleWritePlainItemService : IConsoleWriteItemService
    {
        public void Write(IItem item)
        {
            SysConsole.WriteLine(item.Text);
        }
    }
}
