using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.Intellisense;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;

namespace Commands.Dotnet
{
    public class DotnetIntellisenseProvider : IIntellisenseProvider
    {
        private readonly IFallbackService fallback;
        private readonly IIntellisenseCache cache;
        private readonly HashSet<string> commandsToCache;

        public DotnetIntellisenseProvider(IFallbackService fallback)
        {
            this.fallback = fallback ?? throw new ArgumentNullException(nameof(fallback));
            cache = new IntellisenseCache();

            commandsToCache = new HashSet<string>(
                new string[] {
                    "dotnet ", "dotnet clean ", "dotnet restore ", "dotnet build ",
                    "dotnet msbuild ", "dotnet build-server ", "dotnet run ",
                    "dotnet test ", "dotnet vstest ",
                    "dotnet pack ", "dotnet publish " },
                StringComparer.OrdinalIgnoreCase);
        }

        public async Task<IEnumerable<IItem>> GetHelpAsync(Input input)
        {
            if (input.ParsedInput.RawInput.Length > 50
                || input.ParsedInput.RawInput.Contains('\"')
                || input.ParsedInput.RawInput.Contains('\''))
            {
                return Enumerable.Empty<IItem>();
            }

            string dotnetInput = string.Join(" ", input.ParsedInput.Tokens) + " ";

            if (cache.TryGet(dotnetInput, out var items))
            {
                return items;
            }

            string command = $"dotnet complete \"{dotnetInput}\"";
            var commandResults = await fallback.ProcessIndependentCommandAsync(command);

            if (commandResults == null)
            {
                return Enumerable.Empty<IItem>();
            }

            var newItems = commandResults.Select<string, IItem>(item => Item.Intellisense(item, item.StartsWith("-") ? "option" : "command", null));

            if (commandsToCache.Contains(dotnetInput))
            {
                cache.Set(dotnetInput, newItems);
            }

            return newItems;
        }
    }
}
