using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Xml.Linq;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;

namespace Commands.ReferenceCheck
{
    public class ReferenceCheckCommand : ICommand
    {
        public ICommandResult Execute(CommandContext context)
        {
            var result = ImmutableList<Item>.Empty.ToBuilder();
            var paths = context.GetParameterValues(ParameterKeys.PATH);

            foreach (string strPath in paths)
            {
                TexoPath path = new TexoPath(strPath);

                foreach (string directory in path.GetDirectories())
                {
                    result.AddRange(ProcessFolder(directory));
                }
            }

            if (paths.Count < 1)
            {
                result.AddRange(ProcessFolder("."));
            }

            return new ItemsResult(result.ToImmutable());
        }

        public static Query BuildConfiguration()
        {
            var command = Query.CreateBuilder();

            var parameter = Parameter.CreateBuilder();
            parameter.Key = ParameterKeys.PATH;
            parameter.ArgumentTemplate = InputRegex.WILDCARD_PATH;
            parameter.IsOptional = true;
            parameter.IsRepeatable = true;
            parameter.Documentation.Title = "Directory path";
            parameter.Documentation.Description = "Specify full or relative path(s) to directory with csproj files.";

            command.Key = ReferenceCheckConstants.REF_CHECK;
            command.Representations.AddRange(
                new[] { ReferenceCheckConstants.REF_CHECK, "refcheck", "rcheck", "projcheck" });
            command.Parameters.Add(parameter.ToImmutable());
            command.Documentation.Title = "Project reference check";
            command.Documentation.Description = "Check OLD C# project for duplicate references.";

            return command.ToImmutable();
        }

        private static IEnumerable<Item> ProcessFolder(string folderPath)
        {
            IMarkdownBuilder markdown = new MarkdownBuilder();

            folderPath = Path.GetFullPath(folderPath);
            markdown.Header(folderPath);
            DirectoryInfo directory = new DirectoryInfo(folderPath);

            if (!directory.Exists)
            {
                markdown.Italic("Directory doesn't exist.");
                yield return Item.Markdown(markdown.ToString());
                yield break;
            }

            FileInfo[] projectFiles = directory.GetFiles("*.csproj", SearchOption.AllDirectories);
            markdown.Italic($"{projectFiles.Length} project file(s).");
            yield return Item.Markdown(markdown.ToString());

            foreach (FileInfo projectFile in projectFiles)
            {
                yield return ProcessProjectFile(projectFile);
            }
        }

        private static Item ProcessProjectFile(FileInfo projectFile)
        {
            IMarkdownBuilder markdown = new MarkdownBuilder();
            markdown.Header(projectFile.Name, 2);

            try
            {
                ProcessProjectContent(projectFile, markdown);
            }
            catch (Exception exception)
            {
                markdown.Blockquotes(exception.Message);
            }

            return Item.Markdown(markdown.ToString());
        }

        private static void ProcessProjectContent(FileInfo projectFile, IMarkdownBuilder markdown)
        {
            XDocument content;

            try
            {
                using (Stream fileContent = projectFile.OpenRead())
                {
                    content = XDocument.Load(fileContent);
                }

                if (content.Root == null)
                {
                    markdown.Italic("Invalid content of project file.");
                    return;
                }
            }
            catch (Exception e)
            {
                markdown.Italic("Invalid content of project file: " + e.Message);
                return;
            }

            XNamespace defaultNamespace = content.Root.GetDefaultNamespace();
            ISet<string> references = new HashSet<string>();
            int problemCount = 0;

            foreach (XElement projectReferenceElement in content.Descendants(defaultNamespace + "ProjectReference"))
            {
                XElement projectElement = projectReferenceElement.Element(defaultNamespace + "Project");
                XElement projectNameElement = projectReferenceElement.Element(defaultNamespace + "Name");

                if (projectNameElement == null)
                {
                    markdown.CodeBlock("xml", projectReferenceElement.ToString());
                    continue;
                }

                string projectName = projectNameElement.Value;

                if (projectElement == null)
                {
                    markdown.Italic(projectName + " doesn't have GUID.");
                    markdown.WriteLine();
                    continue;
                }

                string projectId = projectElement.Value;

                if (references.Contains(projectId))
                {
                    markdown.WriteLine(projectName);
                    problemCount++;
                }

                references.Add(projectId);
            }

            markdown.Italic($"{problemCount} problem(s) found in {references.Count} reference(s).");
        }
    }
}
