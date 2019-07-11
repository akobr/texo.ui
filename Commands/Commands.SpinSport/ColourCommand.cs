using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.Text;

namespace Commands.SpinSport
{
    public class ColourCommand : BaseSolutionCommand
    {
        private const string CONFIG_PATH = @"SpinSport.Config\Config\ColourMapping.xml";
        private static readonly Regex rgbRegex = new Regex(@"rgb\(\s*(?<r>\d{1,3})\s*,\s*(?<g>\d{1,3})\s*,\s*(?<b>\d{1,3})\s*\)");
        private static readonly Regex rgbaRegex = new Regex(@"rgba\(\s*(?<r>\d{1,3})\s*,\s*(?<g>\d{1,3})\s*,\s*(?<b>\d{1,3})\s*,\s*(?<a>[\d.]+)\s*\)");

        public ColourCommand(ISolutionDirectoryProvider solutionProvider)
            : base(solutionProvider)
        {
            RegisterQuery(SpinSportConstants.QUERY_LIST, (Func<CommandContext, ICommandResult>)this.List);
            RegisterQuery(SpinSportConstants.QUERY_SET, (Func<CommandContext, ICommandResult>)this.Set);
        }

        private TextResult List(CommandContext context)
        {
            string configPath = CombineWithSolutionPath(CONFIG_PATH);

            if (!File.Exists(configPath))
            {
                return "Configuration hasn't been find.";
            }

            Regex filter = context.GetFilterRegex();
            AnsiStringBuilder builder = new AnsiStringBuilder();

            using (FileStream configFile = File.OpenRead(configPath))
            {
                XDocument doc = XDocument.Load(configFile);
                var workbook = doc.Root;
                var nsSS = workbook.GetNamespaceOfPrefix("ss");
                var worksheet = workbook.Element(nsSS + "Worksheet");
                var table = worksheet.Element(nsSS + "Table");

                foreach (var row in table.Elements(nsSS + "Row").Skip(1))
                {
                    var cells = row.Elements(nsSS + "Cell").ToList();
                    string name = cells[0].Element(nsSS + "Data").Value;
                    string color = cells[1].Element(nsSS + "Data").Value;

                    if (filter.IsMatch(name))
                    {
                        WriteColor(builder, name, color);
                    }
                }
            }

            return builder.ToString();
        }

        private TextResult Set(CommandContext context)
        {
            string configPath = CombineWithSolutionPath(CONFIG_PATH);

            if (!File.Exists(configPath))
            {
                return "Configuration hasn't been find.";
            }

            string name = context.GetParameterValue(SpinSportConstants.PARAMETER_NAME);
            string color = context.GetParameterValue(SpinSportConstants.PARAMETER_VALUE);
            AnsiStringBuilder builder = new AnsiStringBuilder();
            XDocument doc;
            XElement targetRow = null;

            using (FileStream configFile = File.OpenRead(configPath))
            {
                doc = XDocument.Load(configFile);          
            }

            var workbook = doc.Root;
            var nsSS = workbook.GetNamespaceOfPrefix("ss");
            var worksheet = workbook.Element(nsSS + "Worksheet");
            var table = worksheet.Element(nsSS + "Table");

            foreach (var row in table.Elements(nsSS + "Row").Skip(1))
            {
                string rowName = row.Element(nsSS + "Cell").Element(nsSS + "Data").Value;

                if (string.Equals(rowName, name, StringComparison.OrdinalIgnoreCase))
                {
                    targetRow = row;
                    break;
                }

            }

            if (targetRow != null)
            {
                var cells = targetRow.Elements(nsSS + "Cell").ToList();
                cells[1].Element(nsSS + "Data").Value = color ?? string.Empty;
            }
            else
            {
                XElement newRow = new XElement(nsSS + "Row",
                    new XElement(nsSS + "Cell",
                        new XElement(nsSS + "Data",
                            new XAttribute(nsSS + "Type", "String"),
                            name)),
                    new XElement(nsSS + "Cell",
                        new XElement(nsSS + "Data",
                            new XAttribute(nsSS + "Type", "String"),
                            color)));
                table.Add(newRow);
            }

            using (FileStream configFile = File.OpenWrite(configPath))
            {
                doc.Save(configFile);
            }

            WriteColor(builder, name, color);
            return builder.ToString();
        }

        private static void WriteColor(AnsiStringBuilder builder, string colorName, string colorValue)
        {
            WriteColorPreview(builder, ReadColorFromText(colorValue));

            builder.Append(colorName);
            builder.AppendForegroundFormat(ConsoleColor.Gray);
            builder.Append(": ");
            builder.Append(colorValue);
            builder.AppendFormattingReset();
            builder.AppendLine();
        }

        private static (int R, int G, int B) ReadColorFromText(string textColor)
        {
            textColor = textColor.Trim().TrimStart('#');
            int a, r, g, b;

            if (textColor.Length == 8
                && int.TryParse(textColor.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out a)
                && int.TryParse(textColor.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out r)
                && int.TryParse(textColor.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out g)
                && int.TryParse(textColor.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b))
            {
                // TODO: blend the color
                return (r, g, b);
            }
            else if (textColor.Length == 6
                     && int.TryParse(textColor.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out r)
                     && int.TryParse(textColor.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out g)
                     && int.TryParse(textColor.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b))
            {
                return (r, g, b);
            }
            else if(textColor.Length == 3
                     && int.TryParse(textColor.Substring(0, 1) + textColor.Substring(0, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out r)
                     && int.TryParse(textColor.Substring(1, 1) + textColor.Substring(1, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out g)
                     && int.TryParse(textColor.Substring(2, 1) + textColor.Substring(2, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b))
            {
                return (r, g, b);
            }
            else if (rgbRegex.IsMatch(textColor))
            {
                Match match = rgbRegex.Match(textColor);

                if (int.TryParse(match.Groups["r"].Value, out r)
                    && int.TryParse(match.Groups["g"].Value, out g)
                    && int.TryParse(match.Groups["b"].Value, out b))
                {
                    return (r, g, b);
                }

            }
            else if (rgbaRegex.IsMatch(textColor))
            {
                Match match = rgbaRegex.Match(textColor);

                if (int.TryParse(match.Groups["r"].Value, out r)
                    && int.TryParse(match.Groups["g"].Value, out g)
                    && int.TryParse(match.Groups["b"].Value, out b))
                {
                    // TODO: blend the color
                    return (r, g, b);
                }
            }

            return (-1, -1, -1);
        }

        private static void WriteColorPreview(AnsiStringBuilder builder, (int R, int G, int B) color)
        {
            if (color.R < 0 || color.G < 0 || color.B < 0)
            {
                return;
            }

            builder.AppendForegroundFormat((byte)color.R, (byte)color.G, (byte)color.B);
            builder.Append(new string('█', 7));
            builder.AppendFormattingReset();
            builder.Append(" ");
        }
    }
}
