using System;
using System.Linq;
using System.Text;
using BeaverSoft.Texo.Core.Commands;
using System.Text.RegularExpressions;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.Text;
using System.IO;
using System.Xml.Linq;

namespace Commands.SpinSport
{
    public class LocalisationCommand : BaseSolutionCommand
    {
        private const string CONFIG_PATH = @"SpinSport.Client.Localisation\Localisation.xml";

        public LocalisationCommand(ISolutionDirectoryProvider solutionProvider)
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
                var worksheet = workbook.Element(nsSS + "Worksheet");
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
            string[] languages = new[] { "en", "sv", "de", "lv", "no", "fi", "fr", "nl", "cs", "da", "es", "it", "pl", "pt", "ru", "es-mx", "en-in", "hi" };

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

                    if (!filter.IsMatch(name))
                    {
                        continue;
                    }

                    builder.AppendLine(name);

                    for (int i = 1; i < cells.Count; i++)
                    {
                        WriteLanguage(builder, languages[i - 1], cells[i].Element(nsSS + "Data").Value);
                    }

                    builder.AppendLine();
                }
            }

            return builder.ToString();
        }

        // TODO: [P3] Refarctor this and make it generic
        private TextResult Set(CommandContext context)
        {
            string configPath = CombineWithSolutionPath(CONFIG_PATH);

            if (!File.Exists(configPath))
            {
                return "Configuration hasn't been find.";
            }

            string name = context.GetParameterValue(SpinSportConstants.PARAMETER_NAME);
            AnsiStringBuilder builder = new AnsiStringBuilder();
            string[] languages = new[] { "en", "sv", "de", "lv", "no", "fi", "fr", "nl" };
            XElement targetRow = null;
            XDocument doc;

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
                builder.AppendLine(name);

                for (int i = 1; i < cells.Count; i++)
                {
                    var cell = cells[i];
                    string languageCode = languages[i - 1];
                    string languageValue = context.GetParameterFromOption(languageCode) ?? string.Empty;
                    cell.Element(nsSS + "Data").Value = languageValue;
                    WriteLanguage(builder, languageCode, languageValue);
                }
            }
            else
            {
                XElement newRow = new XElement(nsSS + "Row",
                    new XElement(nsSS + "Cell", new XElement(nsSS + "Data", new XAttribute(nsSS + "Type", "String"), name)),
                    new XElement(nsSS + "Cell", new XElement(nsSS + "Data", new XAttribute(nsSS + "Type", "String"), context.GetParameterFromOption(SpinSportConstants.OPTION_EN) ?? string.Empty)),
                    new XElement(nsSS + "Cell", new XElement(nsSS + "Data", new XAttribute(nsSS + "Type", "String"), context.GetParameterFromOption(SpinSportConstants.OPTION_SV) ?? string.Empty)),
                    new XElement(nsSS + "Cell", new XElement(nsSS + "Data", new XAttribute(nsSS + "Type", "String"), context.GetParameterFromOption(SpinSportConstants.OPTION_DE) ?? string.Empty)),
                    new XElement(nsSS + "Cell", new XElement(nsSS + "Data", new XAttribute(nsSS + "Type", "String"), context.GetParameterFromOption(SpinSportConstants.OPTION_LV) ?? string.Empty)),
                    new XElement(nsSS + "Cell", new XElement(nsSS + "Data", new XAttribute(nsSS + "Type", "String"), context.GetParameterFromOption(SpinSportConstants.OPTION_NO) ?? string.Empty)),
                    new XElement(nsSS + "Cell", new XElement(nsSS + "Data", new XAttribute(nsSS + "Type", "String"), context.GetParameterFromOption(SpinSportConstants.OPTION_FI) ?? string.Empty)),
                    new XElement(nsSS + "Cell", new XElement(nsSS + "Data", new XAttribute(nsSS + "Type", "String"), context.GetParameterFromOption(SpinSportConstants.OPTION_FR) ?? string.Empty)),
                    new XElement(nsSS + "Cell", new XElement(nsSS + "Data", new XAttribute(nsSS + "Type", "String"), context.GetParameterFromOption(SpinSportConstants.OPTION_NL) ?? string.Empty)));

                table.Add(newRow);

                builder.AppendLine(name);
                WriteLanguage(builder, SpinSportConstants.OPTION_EN, context.GetParameterFromOption(SpinSportConstants.OPTION_EN) ?? string.Empty);
                WriteLanguage(builder, SpinSportConstants.OPTION_SV, context.GetParameterFromOption(SpinSportConstants.OPTION_SV) ?? string.Empty);
                WriteLanguage(builder, SpinSportConstants.OPTION_DE, context.GetParameterFromOption(SpinSportConstants.OPTION_DE) ?? string.Empty);
                WriteLanguage(builder, SpinSportConstants.OPTION_LV, context.GetParameterFromOption(SpinSportConstants.OPTION_LV) ?? string.Empty);
                WriteLanguage(builder, SpinSportConstants.OPTION_NO, context.GetParameterFromOption(SpinSportConstants.OPTION_NO) ?? string.Empty);
                WriteLanguage(builder, SpinSportConstants.OPTION_FI, context.GetParameterFromOption(SpinSportConstants.OPTION_FI) ?? string.Empty);
                WriteLanguage(builder, SpinSportConstants.OPTION_FR, context.GetParameterFromOption(SpinSportConstants.OPTION_FR) ?? string.Empty);
                WriteLanguage(builder, SpinSportConstants.OPTION_NL, context.GetParameterFromOption(SpinSportConstants.OPTION_NL) ?? string.Empty);
            }

            using (FileStream configFile = File.OpenWrite(configPath))
            {
                doc.Save(configFile);
            }

            return builder.ToString();
        }

        private static void WriteLanguage(AnsiStringBuilder builder, string language, string value)
        {
            builder.AppendForegroundFormat(ConsoleColor.Gray);
            builder.Append($"{language}: ");
            builder.AppendFormattingReset();
            builder.AppendLine(value);
        }
    }
}
