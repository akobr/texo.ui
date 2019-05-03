using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.Intellisense;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;
using CalcEngine;

namespace Commands.Calc
{
    public class CalcCommand : ICommand, ISynchronousIntellisenseProvider
    {
        private const string VARIABLE_RESULT = "result";
        private const string FUNCTION_NUMBER = "DEV_NUMBER";
        private const string FUNCTION_COLOR = "DEV_COLOR";
        private const string FUNCTION_BASE64 = "DEV_BASE64";

        private readonly CalcEngine.CalcEngine engine;

        public CalcCommand()
        {
            engine = new CalcEngine.CalcEngine();

            engine.Variables[VARIABLE_RESULT] = 0;
            engine.RegisterFunction(FUNCTION_NUMBER, 1, int.MaxValue, FunctionNumber);
            engine.RegisterFunction(FUNCTION_COLOR, 1, int.MaxValue, FunctionColor);
            engine.RegisterFunction(FUNCTION_BASE64, 1, FunctionBase64);
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

        private object FunctionBase64(List<Expression> parms)
        {
            string text = parms[0].Evaluate().ToString();

            if (Regex.IsMatch(text, "^[A-Za-z0-9+\\/]+={0,2}$"))
            {
                byte[] data = Convert.FromBase64String(text);
                string decoded = Encoding.UTF8.GetString(data);
                return decoded;
            }
            else
            {
                byte[] data = Encoding.UTF8.GetBytes(text);
                string encoded = Convert.ToBase64String(data);
                return encoded;
            }
        }

        private object FunctionColor(List<Expression> parms)
        {
            StringBuilder result = new StringBuilder();
            var evaluatedParms = parms.Select(par => par.Evaluate()).ToList();
            Color color;

            if (evaluatedParms.Count == 4 && evaluatedParms.All(par => par is int))
            {
                color = Color.FromArgb((int)evaluatedParms[0], (int)evaluatedParms[1], (int)evaluatedParms[2], (int)evaluatedParms[3]);
                result.AppendLine(BuildColorInfo(color));
            }
            else if(evaluatedParms.Count == 3 && evaluatedParms.All(par => par is int))
            {
                color = Color.FromArgb(255, (int)evaluatedParms[0], (int)evaluatedParms[1], (int)evaluatedParms[2]);
                result.AppendLine(BuildColorInfo(color));
            }
            else
            {
                foreach (string textPar in evaluatedParms.Select(par => par.ToString()))
                {
                    color = ReadColorFromText(textPar);

                    if (color.IsEmpty)
                    {
                        result.AppendLine($"\"{textPar}\": invalid format");
                    }
                    else
                    {
                        result.AppendLine(BuildColorInfo(color));
                    }
                }
            }

            return result.ToString();
        }

        private object FunctionNumber(List<Expression> parms)
        {
            StringBuilder result = new StringBuilder();

            foreach (Expression para in parms)
            {
                result.AppendLine(BuildNumberInfo(para));
            }

            return result.ToString();
        }

        private static Color ReadColorFromText(string textColor)
        {
            textColor = textColor.TrimStart('#');
            int a, r, g, b;

            if (textColor.Length == 8
                && int.TryParse(textColor.Substring(0,2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out a)
                && int.TryParse(textColor.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out r)
                && int.TryParse(textColor.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out g)
                && int.TryParse(textColor.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b))
            {
                return Color.FromArgb(a, r, g, b);
            }
            else if (textColor.Length == 6
                     && int.TryParse(textColor.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out r)
                     && int.TryParse(textColor.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out g)
                     && int.TryParse(textColor.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b))
            {
                return Color.FromArgb(r, g, b);
            }
            else
            {
                return Color.FromName(textColor);
            }
        }

        private static string BuildColorInfo(Color color)
        {
            if (color.A < 255)
            {
                color = Blend(color, Color.Black, color.A / 255);
            }
            
            return $"#{Convert.ToString(color.A, 16)}{Convert.ToString(color.R, 16)}{Convert.ToString(color.G, 16)}{Convert.ToString(color.B, 16)}; argb({color.A},{color.R},{color.G},{color.B}); \u001b[38;2;{color.R};{color.G};{color.B}m{(char)222}{new string((char)219, 3)}{(char)221}\u001b[m"; 
        }

        /// <summary>Blends the specified colors together.</summary>
        /// <param name="color">Color to blend onto the background color.</param>
        /// <param name="backColor">Color to blend the other color onto.</param>
        /// <param name="amount">How much of <paramref name="color"/> to keep,
        /// “on top of” <paramref name="backColor"/>.</param>
        /// <returns>The blended colors.</returns>
        private static Color Blend(Color color, Color backColor, double amount)
        {
            byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            return Color.FromArgb(r, g, b);
        }

        private static string BuildNumberInfo(Expression para)
        {
            string value = para.Evaluate().ToString();
            value = value.Trim('\"');

            try
            {
                string @decimal, hex, binary;
                int number = 0;

                if (Regex.IsMatch(value, "^(0|b|0b)[01]+$"))
                {
                    number = Convert.ToInt32(value, 2);
                }
                else if (Regex.IsMatch(value, "^(x|0x)[0-9A-Fa-f]+$"))
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
                return $"dec:{@decimal}; hex:{hex}; bin:{binary};";
            }
            catch (FormatException)
            {
                return $"\"{value}\"; invalid format; [0|b|0b]0101; [x|0x]0A2F";
            }
            catch (Exception anyException)
            {
                return $"\"{value}\"; {anyException.Message};";
            }
        }
    }
}
