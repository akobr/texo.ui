using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Commands.Calc
{
    public class CalcCommand : ICommand, ISimpleIntellisenceSource
    {
        private readonly CalcEngine.CalcEngine engine;

        public CalcCommand()
        {
            engine = new CalcEngine.CalcEngine();
        }

        public static Query BuildConfiguration()
        {
            var command = Query.CreateBuilder();

            command.Key = "calculator";
            command.Representations.AddRange(new[] { "calculator", "calc" });
            command.Documentation.Title = "Calculator";
            command.Documentation.Description = "Cool Excel-like calculator with support for formulas and variables.";

            var expression = Parameter.CreateBuilder();
            expression.Key = "expression";
            expression.ArgumentTemplate = @"\S";
            expression.IsRepeatable = true;
            expression.Documentation.Title = "Expression";
            expression.Documentation.Description = "Expression which will be executed in calculator.";
            
            command.Parameters.Add(expression.ToImmutable());

            return command.ToImmutable();
        }

        public ICommandResult Execute(CommandContext context)
        {
            string expression = string.Join(string.Empty, context.Parameters["expression"].GetValues());

            try
            {
                object result = engine.Evaluate(expression);
                return new TextResult(result.ToString());
            }
            catch (Exception exception)
            {
                return new ErrorTextResult(exception.Message);
            }
        }

        public IEnumerable<IItem> GetHelp(string input)
        {
            foreach (string functionName in engine.Functions.Keys)
            {
                if (!functionName.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                Item helpItem = Item.Plain(functionName);
                helpItem.AddAction(new Link("use", ActionBuilder.InputUpdateUri(functionName)));
                yield return helpItem;
            }
        }
    }
}
