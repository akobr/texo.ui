using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Extensibility.Attributes;
using BeaverSoft.Texo.Core.Result;

[assembly: CommandLibrary]

namespace BeaverSoft.Texo.Commands.Functions
{
    [Command("functions", Representations = "functions func fn dev")]
    [Documentation("Developer functions", "Helpful functions for a developer.")]
    public class FunctionsCommand
    {
        [Query("color", Representations = "color colour")]
        [Parameter("color", IsRepetable = true)]
        [Documentation("Color function", "Shows a color in multiple formats.")]
        public ICommandResult ColorFunction(CommandContext context)
        {
            StringBuilder result = new StringBuilder();
            var evaluatedParms = context.GetParameterValues("color");
            Color color;

            if (evaluatedParms.Count == 4 && evaluatedParms.All(par => int.TryParse(par, out _)))
            {
                color = Color.FromArgb(int.Parse(evaluatedParms[0]), int.Parse(evaluatedParms[1]), int.Parse(evaluatedParms[2]), int.Parse(evaluatedParms[3]));
                result.AppendLine(BuildColorInfo(color));
            }
            else if (evaluatedParms.Count == 3 && evaluatedParms.All(par => int.TryParse(par, out _)))
            {
                color = Color.FromArgb(255, int.Parse(evaluatedParms[0]), int.Parse(evaluatedParms[1]), int.Parse(evaluatedParms[2]));
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

            return new TextResult(result.ToString());
        }

        [Query("number", Representations = "number num")]
        [Parameter("number", IsRepetable = true)]
        [Documentation("Number function", "Shows a in multiple formats.")]
        public ICommandResult Number(CommandContext context)
        {
            StringBuilder result = new StringBuilder();

            foreach (string number in context.GetParameterValues("number"))
            {
                result.AppendLine(BuildNumberInfo(number));
            }

            return new TextResult(result.ToString());
        }

        [Query("base64", Representations = "base64 b64 64")]
        [Parameter("input", IsRepetable = false)]
        [Documentation("Base64 function", "Codes and decodes base64 string.")]
        public ICommandResult Base64(CommandContext context)
        {
            string text = context.GetParameterValue("input");

            if (Regex.IsMatch(text, "^[A-Za-z0-9+\\/]+={0,2}$"))
            {
                byte[] data = Convert.FromBase64String(text);
                string decoded = Encoding.UTF8.GetString(data);
                return new TextResult(decoded);
            }
            else
            {
                byte[] data = Encoding.UTF8.GetBytes(text);
                string encoded = Convert.ToBase64String(data);
                return new TextResult(encoded);
            }
        }

        [Query("hash-md5", Representations = "hash-md5 md5")]
        [Parameter("source", IsRepetable = false)]
        [Documentation("Hash function (MD5)", "Calculates MD5 hash from input text or a file.")]
        public ICommandResult HashMd5(CommandContext context)
        {
            throw new NotImplementedException();
        }

        [Query("hash-sha1", Representations = "hash-sha1 sha1")]
        [Parameter("source", IsRepetable = false)]
        [Documentation("Hash function (SHA1)", "Calculates SHA1 hash from input text or a file.")]
        public ICommandResult HashSha1(CommandContext context)
        {
            throw new NotImplementedException();
        }

        [Query("guid", Representations = "guid")]
        [Documentation("Guid generator", "Generates random GUID and show it in multiple formats.")]
        public ICommandResult Guid(CommandContext context)
        {
            throw new NotImplementedException();
        }

        [Query("random", Representations = "random rnd")]
        [Option("text", Representations = "text txt t")]
        [Option("number", Representations = "number n")]
        [Option("person", Representations = "person p")]
        [Option("address", Representations = "address a")]
        [Option("gps", Representations = "gps g")]
        [Documentation("Random generator", "Generates random text, person information, address, GPS coordinates and numbers.")]
        public ICommandResult Random(CommandContext context)
        {
            throw new NotImplementedException();
        }

        private static string BuildNumberInfo(string value)
        {
            value = value.Trim();

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

        private static Color ReadColorFromText(string textColor)
        {
            textColor = textColor.TrimStart('#');
            int a, r, g, b;

            if (textColor.Length == 8
                && int.TryParse(textColor.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out a)
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
    }
}
