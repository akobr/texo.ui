using BeaverSoft.Texo.Core.Configuration;

namespace BeaverSoft.Texo.Commands.Functions
{
    public static class FunctionsBuilder
    {
        public static Query BuildCommand()
        {
            var command = Query.CreateBuilder();
            command.Key = "functions";
            command.Representations.AddRange(new[] { "functions", "func", "fn", "dev" });
            command.DefaultQueryKey = "list";
            command.Documentation.Title = "Developer functions";
            command.Documentation.Description = "Helpful functions for a developer.";

            var queryList = Query.CreateBuilder();
            queryList.Key = "list";
            queryList.Representations.AddRange(new[] { "list" });
            queryList.Documentation.Title = "List of functions";
            queryList.Documentation.Description = "Shows all available help functions.";

            var queryColor = Query.CreateBuilder();
            queryColor.Key = "color";
            queryColor.Representations.AddRange(new[] { "color", "colour" });
            queryColor.Documentation.Title = "Color function";
            queryColor.Documentation.Description = "Shows a color in multiple formats.";

            var varColor = Parameter.CreateBuilder();
            varColor.Key = "color";
            varColor.ArgumentTemplate = @"(#[a-fA-F0-9]{3,8}|[A-Za-z]+|[0-9]{1,3})";
            varColor.IsRepeatable = true;
            queryColor.Parameters.Add(varColor.ToImmutable());

            var queryNumber = Query.CreateBuilder();
            queryNumber.Key = "number";
            queryNumber.Representations.AddRange(new[] { "number", "num" });
            queryNumber.Documentation.Title = "Number function";
            queryNumber.Documentation.Description = "Shows a number in multiple numerical systems.";

            var varNumber = Parameter.CreateBuilder();
            varNumber.Key = "number";
            varNumber.ArgumentTemplate = @"0?(b|x)?[0-9]+";
            varNumber.IsRepeatable = true;
            queryNumber.Parameters.Add(varNumber.ToImmutable());

            var queryBase64 = Query.CreateBuilder();
            queryBase64.Key = "base64";
            queryBase64.Representations.AddRange(new[] { "base64", "b64", "64" });
            queryBase64.Documentation.Title = "Base64 function";
            queryBase64.Documentation.Description = "Codes and decodes base64 string.";

            var varText = Parameter.CreateBuilder();
            varText.Key = "text";
            queryBase64.Parameters.Add(varText.ToImmutable());

            var queryGuid = Query.CreateBuilder();
            queryGuid.Key = "guid";
            queryGuid.Representations.AddRange(new[] { "guid" });
            queryGuid.Documentation.Title = "Guid generator";
            queryGuid.Documentation.Description = "Generates random GUID and show it in multiple formats.";

            command.Queries.AddRange(new[]
            {
                queryList.ToImmutable(),
                queryColor.ToImmutable(),
                queryNumber.ToImmutable(),
                queryBase64.ToImmutable(),
                queryGuid.ToImmutable(),
            });

            return command.ToImmutable();
        }
    }
}
