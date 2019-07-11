using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Result
{
    public class ItemsResult : ItemsResult<Item>
    {
        public ItemsResult(IImmutableList<Item> content)
            : base(content)
        {
            // no operation
        }

        public ItemsResult(IEnumerable<Item> content)
            : base(content)
        {
            // no operation
        }

        public ItemsResult(params Item[] content)
            : this(ImmutableList.Create(content))
        {
            // no operation
        }
    }

    public class ItemsResult<TItem> : ICommandResult<IImmutableList<TItem>>
        where TItem : IItem
    {
        public ItemsResult(IImmutableList<TItem> content)
        {
            Content = content;
        }

        public ItemsResult(IEnumerable<TItem> content)
        {
            Content = ImmutableList<TItem>.Empty.AddRange(content);
        }

        public ItemsResult(params TItem[] content)
            : this(ImmutableList.Create(content))
        {
            // no operation
        }

        dynamic ICommandResult.Content => Content;

        public IImmutableList<TItem> Content { get; }

        public ResultTypeEnum ResultType { get; set; }

        public virtual Task ExecuteResultAsync()
        {
            return Task.CompletedTask;
        }
    }
}
