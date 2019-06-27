using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Extensibility.Attributes;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.Text;
using Bogus;

[assembly: CommandLibrary]

namespace BeaverSoft.Texo.Commands.Functions
{
    [Command("functions", Representations = "functions func fn dev")]
    [Documentation("Developer functions", "Helpful functions for a developer.")]
    public class FunctionsCommand : InlineIntersectionCommand
    {
        public FunctionsCommand()
        {
            RegisterQueryMethod("list", List);
            RegisterQueryMethod("color", ColorFunction);
            RegisterQueryMethod("number", Number);
            RegisterQueryMethod("base64", Base64);
            RegisterQueryMethod("guid", Guid);
            RegisterQueryMethod("hash-md5", HashMd5);
            RegisterQueryMethod("hash-sha1", HashSha1);
            RegisterQueryMethod("random", Random);
        }

        [Query("list", Representations = "list", IsDefault = true)]
        [Documentation("List of functions", "Shows all available help functions.")]
        public TextResult List(CommandContext context)
        {
            AnsiStringBuilder builder = new AnsiStringBuilder();
            AddFunctionToList(builder, "base64", "Codes and decodes base64 string from input or a file.");
            AddFunctionToList(builder, "color", "Shows a colour(s) in multiple formats. Supported inputs: #[AA]RRGGBB; int[] { [AAA], RRR, GGG, BBB }.");
            AddFunctionToList(builder, "guid", "Generates random GUID and shows it in multiple formats.");
            AddFunctionToList(builder, "md5", "Calculates MD5 hash from input text or a file.");
            AddFunctionToList(builder, "number", "Shows a number in multiple numerical systems (dec, hex, bin). Supported inputs: 42; [0]b101010; [0]x2A.");
            AddFunctionToList(builder, "random", "Generates random text, person information, address, GPS coordinates and numbers.");
            AddFunctionToList(builder, "sha1", "Calculates SHA1 hash from input text or a file.");
            return builder.ToString();
        }

        [Query("color", Representations = "color colour")]
        [Parameter("color", IsRepetable = true)]
        [Documentation("Color function", "Shows a colour(s) in multiple formats. Supported inputs: #[AA]RRGGBB; int[] { [AAA], RRR, GGG, BBB }.")]
        public TextResult ColorFunction(CommandContext context)
        {
            AnsiStringBuilder result = new AnsiStringBuilder();
            var parValues = context.GetParameterValues("color");
            Color color;

            if (parValues.Count == 4 && parValues.All(par => int.TryParse(par, out _)))
            {
                color = Color.FromArgb(int.Parse(parValues[0]), int.Parse(parValues[1]), int.Parse(parValues[2]), int.Parse(parValues[3]));
                result.AppendLine(BuildColorInfo(color));
            }
            else if (parValues.Count == 3 && parValues.All(par => int.TryParse(par, out _)))
            {
                color = Color.FromArgb(255, int.Parse(parValues[0]), int.Parse(parValues[1]), int.Parse(parValues[2]));
                result.AppendLine(BuildColorInfo(color));
            }
            else
            {
                foreach (string textPar in parValues.Select(par => par.ToString()))
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

            RemoveLastLineEnd(result);
            return result.ToString();
        }

        [Query("number", Representations = "number num")]
        [Parameter("number", IsRepetable = true)]
        [Documentation("Number function", "Shows a number in multiple numerical systems (dec, hex, bin). Supported inputs: 42; [0]b101010; [0]x2A.")]
        public TextResult Number(CommandContext context)
        {
            AnsiStringBuilder result = new AnsiStringBuilder();

            foreach (string number in context.GetParameterValues("number"))
            {
                result.AppendLine(BuildNumberInfo(number));
            }

            RemoveLastLineEnd(result);
            return result.ToString();
        }

        [Query("base64", Representations = "base64 b64 64")]
        [Parameter("text", IsRepetable = false)]
        [Documentation("Base64 function", "Codes and decodes base64 string from input or a file.")]
        public TextResult Base64(CommandContext context)
        {
            string text = context.GetParameterValue("text");

            if (text.GetPathType() == PathTypeEnum.File)
            {
                text = File.ReadAllText(text);
            }

            if (Regex.IsMatch(text, "^[A-Za-z0-9+\\/]+={0,2}$"))
            {
                return DecodeBase64(text);
            }
            else
            {
                return EncodeBase64(text);
            }
        }

        [Query("hash-md5", Representations = "hash-md5 md5")]
        [Parameter("source", IsRepetable = false)]
        [Documentation("Hash function (MD5)", "Calculates MD5 hash from input text or a file.")]
        public TextResult HashMd5(CommandContext context)
        {
            return ComputeHash(context, new MD5CryptoServiceProvider());
        }

        [Query("hash-sha1", Representations = "hash-sha1 sha1")]
        [Parameter("source", IsRepetable = false)]
        [Documentation("Hash function (SHA1)", "Calculates SHA1 hash from input text or a file.")]
        public TextResult HashSha1(CommandContext context)
        {
            return ComputeHash(context, new SHA1CryptoServiceProvider());
        }

        [Query("guid", Representations = "guid")]
        [Documentation("Guid generator", "Generates random GUID and shows it in multiple formats.")]
        public TextResult Guid(CommandContext context)
        {
            Guid guid = System.Guid.NewGuid();
            AnsiStringBuilder builder = new AnsiStringBuilder();            
            bool isLetter = false;

            foreach (char character in guid.ToString("N").ToUpperInvariant())
            {
                if (char.IsDigit(character))
                {
                    if (isLetter)
                    {
                        builder.AppendFormattingReset();
                        isLetter = false;
                    }
                }
                else if (!isLetter)
                {
                    builder.AppendForegroundFormat(ConsoleColor.Yellow);
                    isLetter = true;
                }

                builder.Append(character.ToString());
            }

            if (isLetter)
            {
                builder.AppendFormattingReset();
            }

            builder.AppendLine();
            builder.AppendLine(guid.ToString());
            builder.AppendLine(guid.ToString("B").ToUpperInvariant());
            builder.Append(guid.ToString("X"));

            return builder.ToString();
        }

        [Query("random", Representations = "random rnd")]
        [Documentation("Random generator", "Generates random text, person information, address, GPS coordinates and numbers.")]
        public TextResult Random(CommandContext context)
        {
            AnsiStringBuilder builder = new AnsiStringBuilder();
            Faker faker = new Faker("en");

            // Text
            builder.AppendForegroundFormat(ConsoleColor.Gray);
            builder.Append("text");
            builder.AppendFormattingReset();
            builder.AppendLine();
            builder.AppendLine(faker.Lorem.Paragraph(6));

            // Numbers
            builder.AppendLine();
            builder.AppendForegroundFormat(ConsoleColor.Gray);
            builder.Append("numbers");
            builder.AppendFormattingReset();
            builder.AppendLine();
            int integer = faker.Random.Int(0, int.MaxValue);
            double fractional = faker.Random.Double();
            double @decimal = integer + fractional;
            builder.AppendLine($"integer: {integer}    fractional: {fractional}    decimal: {@decimal}");

            // Person
            builder.AppendLine();
            builder.AppendForegroundFormat(ConsoleColor.Gray);
            builder.Append("person");
            builder.AppendFormattingReset();
            builder.AppendLine();
            Person person = faker.Person;
            builder.AppendLine($"{person.FullName} ({person.UserName})");
            builder.AppendLine($"DOB: {person.DateOfBirth.ToShortDateString()}");
            builder.AppendLine($"Phone: {person.Phone}");
            builder.AppendLine($"Email: {person.Email}");
            builder.AppendLine($"Website: {person.Website}");

            // Address
            builder.AppendLine();
            builder.AppendForegroundFormat(ConsoleColor.Gray);
            builder.Append("address");
            builder.AppendFormattingReset();
            builder.AppendLine();
            builder.AppendLine(faker.Address.FullAddress());

            // GPS
            builder.AppendLine();
            builder.AppendForegroundFormat(ConsoleColor.Gray);
            builder.Append("gps");
            builder.AppendFormattingReset();
            builder.AppendLine();
            double latittude = faker.Address.Latitude();
            double longitude = faker.Address.Longitude();
            builder.Append($"{latittude}, {longitude}");

            return builder.ToString();
        }

        private static string EncodeBase64(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(data);
        }

        private static string DecodeBase64(string text)
        {
            byte[] data = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(data);
        }

        private static string ComputeHash(CommandContext context, HashAlgorithm algorithm)
        {
            string source = context.GetParameterValue("source");
            byte[] bytes;

            if (source.GetPathType() == PathTypeEnum.File)
            {
                bytes = File.ReadAllBytes(source);
            }
            else
            {
                bytes = Encoding.UTF8.GetBytes(source);
            }

            byte[] md5 = algorithm.ComputeHash(bytes);
            return BitConverter.ToString(md5);
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
                    value = value.Substring(value.IndexOf('b') + 1);
                    number = Convert.ToInt32(value, 2);
                }
                else if (Regex.IsMatch(value, "^(x|0x)[0-9A-Fa-f]+$")
                         || Regex.IsMatch(value, "[A-Fa-f]+"))
                {
                    value = value.Substring(value.IndexOf('x') + 1);
                    number = Convert.ToInt32(value, 16);
                }
                else
                {
                    number = Convert.ToInt32(value);
                }

                AnsiStringBuilder builder = new AnsiStringBuilder();
                @decimal = Convert.ToString(number, 10);
                hex = Convert.ToString(number, 16);
                binary = Convert.ToString(number, 2);
                builder.AppendForegroundFormat(ConsoleColor.Gray);
                builder.Append("dec:");
                builder.AppendFormattingReset();
                builder.Append($" {@decimal,-10} ");
                builder.AppendForegroundFormat(ConsoleColor.Gray);
                builder.Append("hex:");
                builder.AppendFormattingReset();
                builder.Append($" {hex,7} ");
                builder.AppendForegroundFormat(ConsoleColor.Gray);
                builder.Append("bin:");
                builder.AppendFormattingReset();
                builder.Append($" {binary,22}");
                return builder.ToString();
            }
            catch (FormatException)
            {
                return $"\"{value}\": invalid format; [0|b|0b]0101; [x|0x]0A2F";
            }
            catch (Exception anyException)
            {
                return $"\"{value}\": {anyException.Message}";
            }
        }

        private static Color ReadColorFromText(string textColor)
        {
            textColor = textColor.TrimStart('#');
            int a, r, g, b;

            if (textColor.Length == 8
                && int.TryParse(textColor.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out a)
                && int.TryParse(textColor.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out r)
                && int.TryParse(textColor.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out g)
                && int.TryParse(textColor.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b))
            {
                return Color.FromArgb(a, r, g, b);
            }
            else if (textColor.Length == 6
                     && int.TryParse(textColor.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out r)
                     && int.TryParse(textColor.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out g)
                     && int.TryParse(textColor.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b))
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
            Color blendedColor = color;

            if (color.A < 255)
            {
                blendedColor = Blend(color, Color.Black, color.A / 255.0);
            }

            AnsiStringBuilder builder = new AnsiStringBuilder();
            builder.AppendForegroundFormat(blendedColor.R, blendedColor.G, blendedColor.B);
            builder.Append(new string('█', 7));
            builder.AppendFormattingReset();
            builder.Append(" ");
            builder.Append($"#{GetHexColorPart(color.A)}{GetHexColorPart(color.R)}{GetHexColorPart(color.G)}{GetHexColorPart(color.B)} ");
            builder.Append($"#{GetHexColorPart(blendedColor.R)}{GetHexColorPart(blendedColor.G)}{GetHexColorPart(blendedColor.B)} ");
            builder.Append(string.Format("{0,-22} ", $"argb({color.A},{color.R},{color.G},{color.B})"));
            builder.Append(string.Format("{0,-17}", $"rgb({blendedColor.R},{blendedColor.G},{blendedColor.B})"));
            builder.AppendLine();
            return builder.ToString();
        }

        private static string GetHexColorPart(byte value)
        {
            string part = Convert.ToString(value, 16);

            if (part.Length == 1)
            {
                part += "0";
            }

            return part;
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

        private static void AddFunctionToList(AnsiStringBuilder builder, string function, string description)
        {
            builder.AppendBoldFormat();
            builder.Append(function);
            builder.AppendFormattingReset();
            builder.Append(": ");
            builder.AppendLine(description);
        }

        private static void RemoveLastLineEnd(AnsiStringBuilder builder)
        {
            if (builder.Length < 1)
            {
                return;
            }

            builder.Remove(builder.Length - Environment.NewLine.Length, Environment.NewLine.Length);
        }
    }
}
