using Markdig;
using Markdig.Extensions.EmphasisExtras;
using Markdig.Syntax;

using Markdigger = Markdig.Markdown;

namespace BeaverSoft.Texo.Core.Services
{
    public class MarkdownService : IMarkdownService
    {
        private readonly MarkdownPipeline pipeline;

        public MarkdownService()
        {
            pipeline = new MarkdownPipelineBuilder()
                .UseAutoLinks()
                .UseEmphasisExtras(EmphasisExtraOptions.Default)
                .UseTaskLists()
                .UseCustomContainers()
                .UseFigures()
                .UseFooters()
                .UseCitations()
                .Build();
        }

        public MarkdownPipeline Pipeline => pipeline;

        public string Normalise(string text)
        {
            return Markdigger.Normalize(text, null, pipeline);
        }

        public string ToHtml(string text)
        {
            return Markdigger.ToHtml(text, pipeline);
        }

        public string ToPlainText(string text)
        {
            return Markdigger.ToPlainText(text, pipeline);
        }

        public MarkdownDocument Parse(string text)
        {
            return Markdigger.Parse(text, pipeline);
        }
    }
}
