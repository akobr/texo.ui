using System;
using System.Collections.Immutable;
using System.IO;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Commands.FileManager.Operations
{
    internal static class OperationMethods
    {
        internal static string GetTargetDirectory(this CommandContext context)
        {
            if (!context.HasParameter(FileManagerParameters.SIMPLE_PATH))
            {
                return Environment.CurrentDirectory;
            }

            string targetPath = context.GetParameterValue(FileManagerParameters.SIMPLE_PATH);

            if (File.Exists(targetPath))
            {
                return Path.GetDirectoryName(targetPath);
            }

            return targetPath;
        }

        internal static string GetTargetFile(this CommandContext context)
        {
            return context.GetParameterValue(FileManagerParameters.SIMPLE_PATH);
        }

        internal static bool IsEmpty(this IStageService stage)
        {
            return stage.GetPaths().Count < 1;
        }

        internal static bool HasLobby(this IStageService stage)
        {
            return !string.IsNullOrEmpty(stage.GetLobby());
        }

        internal static bool TryGetPaths(this IStageService stage, out IImmutableList<string> paths)
        {
            paths = stage.GetPaths();
            return paths.Count > 0;
        }

        internal static void CreateDirectory(this string targetPath)
        {
            if (Directory.Exists(targetPath))
            {
                return;
            }

            Directory.CreateDirectory(targetPath);
        }
    }
}
