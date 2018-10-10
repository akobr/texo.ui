using BeaverSoft.Texo.Core.View;
using Markdig;

namespace BeaverSoft.Texo.Core.Services
{
    public class MarkDownService : IMarkDownService
    {
        public string Normalise(string text)
        {
            return Markdown.Normalize(text);
        }

        public string ToHtml(string text)
        {
            return Markdown.ToHtml(text);
        }

        public string ToPlainText(string text)
        {
            return Markdown.ToPlainText(text);
        }
    }
}
