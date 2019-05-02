using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.Intellisense;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;
using CalcEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Commands.Calc
{
    public class CalcCommand : ICommand, ISynchronousIntellisenseProvider
    {
        private const string VARIABLE_RESULT = "result";
        private const string FUNCTION_NUMBER = "DEV.NUMBER";
        private const string FUNCTION_COLOR = "DEV.COLOR";

        private readonly CalcEngine.CalcEngine engine;

        public CalcCommand()
        {
            engine = new CalcEngine.CalcEngine();

            engine.Variables[VARIABLE_RESULT] = 0;
            engine.RegisterFunction(FUNCTION_NUMBER, 1, FunctionNumber);
        }

        private object FunctionNumber(List<Expression> parms)
        {
            StringBuilder result = new StringBuilder();

            foreach (Expression para in parms)
            {
                string value = para.Evaluate().ToString();
                string @decimal, hex, binary;
                int number = 0;

                if (Regex.IsMatch(value, "^(0|b|0b)[01]+$"))
                {
                    number = Convert.ToInt32(value, 2);
                }
                else if(Regex.IsMatch(value, "^(0|x|0x)[0-9A-Fa-f]+$"))
                {
                    number = Convert.ToInt32(value, 16);
                }
                else
                {
                    number = Convert.ToInt32(value);
                }

                @decimal = Convert.ToString(number, 10);
                hex = Convert.ToString(number, 16);
                binary = Convert.ToString(number, 2);

                result.AppendLine($"\"{value}\"; dec:{@decimal}; hex:{hex}; bin:{binary};");
            }

            return result.ToString();
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
                engine.Variables[VARIABLE_RESULT] = result;
                return new TextResult(result.ToString());
            }
            catch (Exception exception)
            {
                return new ErrorTextResult(exception.Message);
            }
        }

        public IEnumerable<IItem> GetHelp(Input input)
        {
            string lastToken = input.ParsedInput.Tokens.Last();

            foreach (string functionName in engine.Functions.Keys)
            {
                if (!functionName.StartsWith(lastToken, StringComparison.OrdinalIgnoreCase))
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
