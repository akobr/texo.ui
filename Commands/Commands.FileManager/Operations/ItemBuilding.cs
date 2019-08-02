using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Commands.FileManager.Operations
{
    public static class ItemBuilding
    {
        public static Item BuildMissingItem(string source)
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            builder.Header(source);
            builder.Italic("Item doesn't exist.");
            return Item.AsMarkdown(builder.ToString());
        }
    }
}
