using System;
using System.Collections.Generic;
using System.Linq;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.Intellisense;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;

namespace Commands.Calc
{
    public class CalcCommand : ICommand, ISynchronousIntellisenseProvider
    {
        private const string VARIABLE_RESULT = "result";

        private readonly CalcEngine.CalcEngine engine;

        public CalcCommand()
        {
            engine = new CalcEngine.CalcEngine();
            engine.Variables[VARIABLE_RESULT] = 0;
        }

        public static Query BuildConfiguration()
        {
            var command = Query.CreateBuilder();

            command.Key = "calculator";
            command.Representations.AddRange(new[] { "calculator", "calc" });
            command.Documentation.Title = "Calculator";
            command.Documentation.Description = "Cool Excel-like calculator with support for formulas and variables.";

            var variableOption = Option.CreateBuilder();
            variableOption.Key = "variable";
            variableOption.Representations.AddRange(new[] { "variable", "var", "v" });
            variableOption.Documentation.Title = "To-variable";
            variableOption.Documentation.Description = "Saves result to specified variable.";

            var variableName = Parameter.CreateBuilder();
            variableName.Key = "variableName";
            variableName.ArgumentTemplate = @"^[a-zA-z_][a-zA-Z0-9_]*$";
            variableName.IsRepeatable = false;
            variableName.Documentation.Title = "Variable name";
            variableName.Documentation.Description = "Name of target variable.";

            variableOption.Parameters.Add(variableName.ToImmutable());

            var expression = Parameter.CreateBuilder();
            expression.Key = "expression";
            expression.ArgumentTemplate = @"^.+$";
            expression.IsRepeatable = true;
            expression.Documentation.Title = "Expression";
            expression.Documentation.Description = "Expression which will be executed in calculator.";

            command.Options.Add(variableOption.ToImmutable());
            command.Parameters.Add(expression.ToImmutable());

            return command.ToImmutable();
        }

        public ICommandResult Execute(CommandContext context)
        {
            string expression = string.Join(string.Empty, context.Parameters["expression"].GetValues());

            try
            {
                object result = engine.Evaluate(expression);
                engine.Variables[VARIABLE_RESULT] = result;

                if (context.HasOption("variable"))
                {
                    var variableContext = context.GetOption("variable");
                    string variableName = variableContext.GetParameterValue("variableName");
                    engine.Variables[variableName] = result;
                    return new TextResult($"{variableName} = {result}");
                }
                else
                {
                    return new TextResult(result.ToString());
                }
            }
            catch (Exception exception)
            {
                return new ErrorTextResult(exception.Message);
            }
        }

        public IEnumerable<IItem> GetHelp(Input input)
        {
            string lastToken = input.ParsedInput.Tokens.Last();

            foreach (var variablePair in engine.Variables)
            {
                if (!variablePair.Key.StartsWith(lastToken, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                yield return Item.AsIntellisense(variablePair.Key, variablePair.Key, "variable", variablePair.Value.ToString());
            }

            foreach (string functionName in engine.Functions.Keys)
            {
                if (!functionName.StartsWith(lastToken, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                yield return Item.AsIntellisense(functionName, "function", string.Empty);
            }
        }
    }
}
