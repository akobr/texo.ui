using BeaverSoft.Texo.Core.Configuration;

namespace Commands.Dotnet
{
    public static class DotnetBuilder
    {
        public static Query BuildCommand()
        {
            var command = Query.CreateBuilder();
            command.Key = "dotnet";
            command.Documentation.Title = "dotnet";
            command.Documentation.Description = "A tool for managing .NET source code and binaries.";

            return command.ToImmutable();
        }
    }
}
