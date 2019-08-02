using System;
using System.Collections.Generic;
using System.Linq;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.Inputting.Tree;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Intellisense
{
    public class CommandDefinitionIntellisenseProvider : ISynchronousIntellisenseProvider
    {
        private readonly ICommandManagementService commandManagement;
        private InputTree tree;

        public CommandDefinitionIntellisenseProvider(ICommandManagementService commandManagement)
        {
            this.commandManagement = commandManagement ?? throw new ArgumentNullException(nameof(commandManagement));
        }

        public void SetTree(InputTree tree)
        {
            this.tree = tree;
        }

        public IEnumerable<IItem> GetHelp(Input input)
        {
            QueryNode query = tree.Root;

            foreach (Token token in input.Tokens)
            {
                if (token.Type != TokenTypeEnum.Query
                    || !query.Queries.ContainsKey(token.Input))
                {
                    break;
                }

                query = query.Queries[token.Input];
            }

            foreach (Query subQuery in query.Query.Queries.OrderBy(q => q.Key))
            {
                yield return Item.AsIntellisense(subQuery.GetMainRepresentation(), "query", subQuery.Documentation.Description);
            }

            foreach (Option subOption in query.Query.Options.OrderBy(o => o.Key))
            {
                string representation = subOption.GetMainRepresentation();
                yield return Item.AsIntellisense($"--{representation}", "option", subOption.Documentation.Description);
            }

            if (!commandManagement.HasCommand(input.Context.Key))
            {
                yield break;
            }
        }
    }
}
