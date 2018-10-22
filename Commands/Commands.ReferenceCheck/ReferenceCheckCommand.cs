using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Xml.Linq;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Model.View;
using BeaverSoft.Texo.Core.Result;

namespace Commands.ReferenceCheck
{
    public class ReferenceCheckCommand : ICommand
    {
        public ICommandResult Execute(ICommandContext context)
        {
            var result = ImmutableList<Item>.Empty.ToBuilder();

            foreach (string path in context.Parameters[ParameterKeys.PATH].GetValues())
            {
                result.AddRange(ProcessFolder(path));
            }

            return new ItemsResult(result.ToImmutable());
        }

        private IEnumerable<Item> ProcessFolder(string folderPath)
        {
            MarkdownBuilder markdown = new MarkdownBuilder();

            folderPath = Path.GetFullPath(folderPath);
            markdown.Header(folderPath);
            DirectoryInfo directory = new DirectoryInfo(folderPath);

            if (!directory.Exists)
            {
                markdown.Italic("Directory doesn't exist.");
                yield return new Item(markdown.ToString());
                yield break;
            }

            FileInfo[] projectFiles = directory.GetFiles("*.csproj", SearchOption.AllDirectories);
            markdown.Italic($"{projectFiles.Length} project file(s).");
            yield return new Item(markdown.ToString());

            foreach (FileInfo projectFile in projectFiles)
            {
                yield return ProcessProjectFile(projectFile);
            }
        }

        private static Item ProcessProjectFile(FileInfo projectFile)
        {
            MarkdownBuilder markdown = new MarkdownBuilder();
            markdown.Header(projectFile.Name, 2);
            XDocument content;

            using (Stream fileContent = projectFile.OpenRead())
            {
                content = XDocument.Load(fileContent);
            }

            XNamespace defaultNamespace = content.Root.GetDefaultNamespace();
            ISet<string> references = new HashSet<string>();

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
                }

                references.Add(projectId);
            }

            return new Item(markdown.ToString());
        }
    }
}
