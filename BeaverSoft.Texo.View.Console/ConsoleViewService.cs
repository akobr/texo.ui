using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Model.View;
using BeaverSoft.Texo.Core.View;

using SysConsole = System.Console;

namespace BeaverSoft.Texo.View.Console
{
    public class ConsoleViewService : IViewService
    {
        private readonly IConsoleWriteItemService writer;
        private readonly ISettingService setting;

        public ConsoleViewService(
            IConsoleWriteItemService writer,
            ISettingService setting)
        {
            this.writer = writer;
            this.setting = setting;
        }

        public void Render(IInput input)
        {
            // TODO
        }

        public void Render(IImmutableList<IItem> items)
        {
            foreach (IItem item in items)
            {
                writer.Write(item);
            }
        }

        public void RenderIntellisence(IImmutableList<IItem> items)
        {
            // TODO
        }

        public void RenderProgress(IProgress progress)
        {
            // TODO
        }

        public void UpdateCurrentDirectory(string directoryPath)
        {
            if (!setting.Configuration.Ui.ShowWorkingPathAsPrompt)
            {
                return;
            }

            TexoConsole.WritePrompt(directoryPath);
        }

        public void Update(string key, IItem item)
        {
            // TODO
        }

        public void Dispose()
        {
        }
    }
}