using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.Text;

namespace Commands.SpinSport
{
    class ColourUsageCommand : BaseSolutionCommand
    {
        private const string CONFIG_PATH = @"SpinSport.Config\Config\Theme.xml";
        private static readonly string[] brands = new[] { "showcase", "betway", "betway-dark", "betway-new", "betway-africa", "betway-africa-mobile" };

        public ColourUsageCommand(ISolutionDirectoryProvider solutionProvider)
            : base(solutionProvider)
        {
            RegisterQueryMethod(SpinSportConstants.QUERY_LIST, List);
            RegisterQueryMethod(SpinSportConstants.QUERY_GET, Get);
            RegisterQueryMethod(SpinSportConstants.QUERY_SET, Set);
        }

        private TextResult List(CommandContext context)
        {
            string configPath = CombineWithSolutionPath(CONFIG_PATH);

            if (!File.Exists(configPath))
            {
                return "Configuration hasn't been find.";
            }

            Regex filter = context.GetFilterRegex();
            StringBuilder builder = new StringBuilder();

            using (FileStream configFile = File.OpenRead(configPath))
            {
                XDocument doc = XDocument.Load(configFile);
                var workbook = doc.Root;
                var nsSS = workbook.GetNamespaceOfPrefix("ss");
                var worksheet = workbook.Elements(nsSS + "Worksheet").FirstOrDefault(w => (string)w.Attribute(nsSS + "Name") == "ColourUsage");
                var table = worksheet.Element(nsSS + "Table");

                foreach (var row in table.Elements(nsSS + "Row").Skip(1))
                {
                    var cells = row.Elements(nsSS + "Cell").ToList();
                    string name = cells[0].Element(nsSS + "Data").Value;
                    string color = cells[1].Element(nsSS + "Data").Value;

                    if (filter.IsMatch(name))
                    {
                        builder.AppendLine(name);
                    }
                }
            }

            return builder.ToString();
        }

        private TextResult Get(CommandContext context)
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
                var worksheet = workbook.Elements(nsSS + "Worksheet").FirstOrDefault(w => (string)w.Attribute(nsSS + "Name") == "ColourUsage");
                var table = worksheet.Element(nsSS + "Table");

                foreach (var row in table.Elements(nsSS + "Row").Skip(1))
                {
                    string name = row.Element(nsSS + "Cell").Element(nsSS + "Data").Value;

                    if (filter.IsMatch(name))
                    {
                        WriteColorUsage(builder, row, nsSS);
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
            string[] brandValues = new[]
            {
                context.GetParameterFromOption(SpinSportConstants.OPTION_SHOWCASE) ?? string.Empty,
                context.GetParameterFromOption(SpinSportConstants.OPTION_BETWAY) ?? string.Empty,
                context.GetParameterFromOption(SpinSportConstants.OPTION_BETWAY_DARK) ?? string.Empty,
                context.GetParameterFromOption(SpinSportConstants.OPTION_BETWAY_NEW) ?? string.Empty,
                context.GetParameterFromOption(SpinSportConstants.OPTION_BETWAY_AFRICA) ?? string.Empty,
                context.GetParameterFromOption(SpinSportConstants.OPTION_BETWAY_AFRICA_MOBILE) ?? string.Empty
            };

            XDocument doc;
            XElement targetRow = null;
            List<XElement> cells = new List<XElement>();

            using (FileStream configFile = File.OpenRead(configPath))
            {
                doc = XDocument.Load(configFile);
            }

            var workbook = doc.Root;
            var nsSS = workbook.GetNamespaceOfPrefix("ss");
            var worksheet = workbook.Elements(nsSS + "Worksheet").FirstOrDefault(w => (string)w.Attribute(nsSS + "Name") == "ColourUsage");
            var table = worksheet.Element(nsSS + "Table");

            foreach (var row in table.Elements(nsSS + "Row").Skip(1))
            {
                cells = row.Elements(nsSS + "Cell").ToList();
                string rowName = cells[0].Element(nsSS + "Data").Value;

                if (string.Equals(rowName, name, StringComparison.OrdinalIgnoreCase))
                {
                    targetRow = row;
                    break;
                }
            }

            if (targetRow != null)
            {
                for (int i = 0; i < brandValues.Length; i++)
                {
                    int cellIndex = i + 1;
                    string value = brandValues[i];

                    if (cellIndex >= targetRow.Elements().Count())
                    {
                        targetRow.Add(
                            new XElement(nsSS + "Cell",
                                new XElement(nsSS + "Data",
                                    new XAttribute(nsSS + "Type", "String"),
                                    value)));
                        continue;
                    }

                    var cell = targetRow.Elements().Skip(cellIndex).First();
                    var attribute = cell.Attribute(nsSS + "Index");

                    if (attribute != null && int.TryParse(attribute.Value, out int realCellIndex))
                    {
                        if (realCellIndex > cellIndex)
                        {
                            cell.AddBeforeSelf(new XElement(nsSS + "Cell",
                                new XElement(nsSS + "Data",
                                    new XAttribute(nsSS + "Type", "String"),
                                    value)));
                            continue;
                        }

                        attribute.Remove();
                    }

                    cell.Element(nsSS + "Data").Value = value;
                }
            }
            else
            {
                targetRow = new XElement(nsSS + "Row",
                    new XElement(nsSS + "Cell",
                        new XElement(nsSS + "Data",
                            new XAttribute(nsSS + "Type", "String"),
                            name)));

                foreach (string brandValue in brandValues)
                {
                    targetRow.Add(new XElement(nsSS + "Cell",
                        new XElement(nsSS + "Data",
                            new XAttribute(nsSS + "Type", "String"),
                            brandValue)));
                }

                table.Add(targetRow);
            }

            using (FileStream configFile = File.OpenWrite(configPath))
            {
                doc.Save(configFile);
            }

            AnsiStringBuilder builder = new AnsiStringBuilder();
            WriteColorUsage(builder, targetRow, nsSS);
            return builder.ToString();
        }

        private static void WriteColorUsage(AnsiStringBuilder builder, XElement row, XNamespace nsSS)
        {
            var cells = row.Elements(nsSS + "Cell").ToList();
            builder.AppendLine(cells[0].Element(nsSS + "Data").Value);

            int index = 0;
            foreach (var cell in cells.Skip(1))
            {
                var attributeIndex = cell.Attribute(nsSS + "Index");
                if (attributeIndex != null && int.TryParse(attributeIndex.Value, out int excelIndex))
                {
                    index = excelIndex - 2;
                }

                if (index >= brands.Length)
                {
                    break;
                }

                builder.AppendForegroundFormat(ConsoleColor.Gray);
                builder.Append($"{brands[index++]}: ");
                builder.AppendFormattingReset();
                builder.AppendLine(cell.Element(nsSS + "Data").Value);
            }

            builder.AppendLine();
        }
    }
}
