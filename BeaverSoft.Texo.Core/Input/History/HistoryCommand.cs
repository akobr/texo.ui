using System.Text;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Result;

namespace BeaverSoft.Texo.Core.Input.History
{
    public class HistoryCommand : ICommand
    {
        private readonly IInputHistoryService history;

        public HistoryCommand(IInputHistoryService history)
        {
            this.history = history;
        }

        public ICommandResult Execute(CommandContext context)
        {
            StringBuilder result = new StringBuilder();

            foreach (IHistoryItem historyItem in history.GetHistory())
            {
                result.AppendLine(historyItem.Input.ParsedInput.RawInput);
            }

            return new TextResult(result.ToString());
        }
    }
}
