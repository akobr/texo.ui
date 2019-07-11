using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Markdown;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.View;
using Commands.SpinSport.Code.Model;
using Newtonsoft.Json;

namespace Commands.SpinSport.Code
{
    public class CodeCommand : AsyncCommand<Item.Markdown>
    {
        protected override async Task<Item.Markdown> ExecuteAsync(CommandContext context)
        {
            string endpointUrl = BuildEndpointUrl(context);
            Result result;

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(endpointUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return Markdown.Italic("Grats! You broke it, kid.");
                }

                using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                using (TextReader reader = new StreamReader(contentStream))
                using (JsonReader jReader = new JsonTextReader(reader))
                {
                    result = JsonSerializer.CreateDefault().Deserialize<Result>(jReader);
                }
            }

            return BuildOutputList(result);
        }

        private string BuildEndpointUrl(CommandContext context)
        {
            string fullSearchTerm = string.Join(" ", context.GetParameterValues(SpinSportConstants.PARAMETER_SEARCH_TERM));
            return $"http://search.clientdoc.ludologic.com/api/v1/search?type=ref&query={Uri.EscapeDataString(fullSearchTerm)}";
        }

        private Item.Markdown BuildOutputList(Result result)
        {
            MarkdownBuilder builder = new MarkdownBuilder();

            if (result?.Records == null || result.Records.Count < 1)
            {
                builder.Italic("Oh snap! No results for this ridiculous search term, please try it later when someone implements it.");
            }
            else
            {
                foreach (Record record in result.Records)
                {
                    builder.Bullet();
                    builder.Link(record.Document.Title, record.Document.Href);
                }
            }

            return builder.ToString();
        }
    }
}
