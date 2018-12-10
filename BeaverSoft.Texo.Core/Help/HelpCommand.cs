using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;

namespace BeaverSoft.Texo.Core.Help
{
    class HelpCommand : InlineIntersectionCommand
    {
        private ISettingService setting;

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
            throw new NotImplementedException();
        }

        private ICommandResult List(CommandContext context)
        {
            String

            foreach (Query command in setting.Configuration.Runtime.Commands.OrderBy(c => c.Key))
            {
                
            }
        }
    }
}
