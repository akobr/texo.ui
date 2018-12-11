using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Model.Text;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Help
{
    class HelpCommand : InlineIntersectionCommand
    {
        private readonly ISettingService setting;

        public HelpCommand(ISettingService setting)
        {
            this.setting = setting ?? throw new ArgumentNullException(nameof(setting));

            RegisterQueryMethod(HelpNames.QUERY_INFO, Info);
            RegisterQueryMethod(HelpNames.QUERY_TREE, Tree);
            RegisterQueryMethod(HelpNames.QUERY_LIST, List);
        }

        private ICommandResult Info(CommandContext context)
        {
            throw new NotImplementedException();
        }

        private ICommandResult Tree(CommandContext context)
        {
            CommandTextTreeBuilder builder = new CommandTextTreeBuilder();
            return new ItemsResult(Item.Plain(builder.BuildTree(setting.Configuration.Runtime.Commands[0])));
        }

        private ICommandResult List(CommandContext context)
        {
            List result = new List();

            foreach (Query command in setting.Configuration.Runtime.Commands.OrderBy(c => c.Key))
            {
                result = result.AddItem(new ListItem(
                    new Span(
                        new Strong(command.GetMainRepresentation()),
                        new PlainText(" - "),
                        new Italic(command.Documentation.Description))));
            }

            return new ItemsResult<ModeledItem>(new ModeledItem(result));
        }
    }
}
