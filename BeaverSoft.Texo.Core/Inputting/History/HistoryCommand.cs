using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.Text;

namespace BeaverSoft.Texo.Core.Inputting.History
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
            AnsiStringBuilder result = new AnsiStringBuilder();

            foreach (IHistoryItem historyItem in history.GetHistory())
            {
                string rawInput = historyItem.Input.ParsedInput.RawInput;
                result.AppendLink(rawInput, ActionBuilder.InputSetUri(rawInput));
                result.AppendLine();
            }

            return new TextResult(result.ToString());
        }
    }
}
