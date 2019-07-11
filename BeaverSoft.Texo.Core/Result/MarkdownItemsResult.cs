using System.Collections.Generic;
using System.Linq;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Result
{
    public class MarkdownItemsResult : ItemsResult
    {
        public MarkdownItemsResult(IEnumerable<string> content)
            : base(content.Select(str => new Item(str, TextFormatEnum.Markdown)))
        {
            // no operation
        }

        public MarkdownItemsResult(params string[] content)
            : base(content.Select(str => new Item(str, TextFormatEnum.Markdown)))
        {
            // no operation
        }
    }
}
