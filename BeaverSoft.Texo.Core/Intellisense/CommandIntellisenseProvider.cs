using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Intellisense
{
    public class CommandIntellisenseProvider : IIntellisenseProvider
    {
        private const int MAXIMUM_ITEMS_COUNT = 20;
        private readonly ICommandManagementService commandManagement;

        public CommandIntellisenseProvider(ICommandManagementService commandManagement)
        {
            this.commandManagement = commandManagement ?? throw new ArgumentNullException(nameof(commandManagement));
        }

        public async Task<IEnumerable<IItem>> GetHelpAsync(Input input)
        {
            IEnumerable<IItem> helpItems = Enumerable.Empty<IItem>();
            ICommand command = commandManagement.BuildCommand(input.Context.Key);

            if (command == null)
            {
                return helpItems;
            }

            switch (command)
            {
                case IIntellisenseProvider asynchronousProvider:
                    helpItems = await asynchronousProvider.GetHelpAsync(input);
                    break;

                case ISynchronousIntellisenseProvider synchronousProvider:
                    helpItems = synchronousProvider.GetHelp(input);
                    break;
            }

            return helpItems.Take(MAXIMUM_ITEMS_COUNT);
        }
    }
}
