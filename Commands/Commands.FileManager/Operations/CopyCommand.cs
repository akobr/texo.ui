using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Commands.FileManager.Operations
{
    public class CopyCommand : ICommand
    {
        private readonly IStageService stage;

        public CopyCommand(IStageService stage)
        {
            this.stage = this.stage ?? throw new ArgumentNullException(nameof(stage));
        }

        public ICommandResult Execute(CommandContext context)
        {
            string targetDir = context.GetTargetDirectory();

            if (!stage.TryGetPaths(out var paths))
            {
                return new TextResult("The stage is empty.");
            }

            targetDir.CreateDirectory();
            bool flatten = context.IsFlatten() || !stage.HasLobby();

            if (flatten)
            {
                return FlattenCopy();
            }

            return Copy();
        }

        private ICommandResult Copy()
        {

        }

        private ICommandResult FlattenCopy(string targetDirectory, IEnumerable<string> paths, bool overrideFiles, bool onlyPreview)
        {
            var result = ImmutableList<Item>.Empty.ToBuilder();

            foreach (string path in paths)
            {
                if (!File.Exists(path))
                {
                    continue;
                }

                MarkdownBuilder itemBuilder = new MarkdownBuilder();
                string fileName = Path.GetFileName(path);
                string targetPath = Path.Combine(targetDirectory, fileName);

                if (File.Exists(targetPath))
                {
                    if (overrideFiles)
                    {
                        itemBuilder.WriteLine("File overriden.");
                    }
                    else
                    {
                        itemBuilder.WriteLine("File not copied, already exists.");
                    }
                }

                if (!onlyPreview)
                {
                    File.Copy(path, targetPath, overrideFiles);
                }
            }
        }
    }
}
