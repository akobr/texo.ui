using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Result
{
    public class ItemsResult : ItemsResult<Item>
    {
        public ItemsResult(ResultTypeEnum resultType, IImmutableList<Item> content)
            : base(resultType, content)
        {
            // no operation
        }
        public ItemsResult(IImmutableList<Item> content)
            : this(ResultTypeEnum.Success, content)
        {
            // no operation
        }

        public ItemsResult(params Item[] content)
            : this(ResultTypeEnum.Success, ImmutableList.Create<Item>(content))
        {
            // no operation
        }
    }

    public class ItemsResult<TItem> : ICommandResult<IImmutableList<TItem>>
        where TItem : IItem
    {
        public ItemsResult(ResultTypeEnum resultType, IImmutableList<TItem> content)
        {
            ResultType = resultType;
            Content = content;
        }
        public ItemsResult(IImmutableList<TItem> content)
            : this(ResultTypeEnum.Success, content)
        {
            // no operation
        }

        public ItemsResult(params TItem[] content)
            : this(ResultTypeEnum.Success, ImmutableList.Create<TItem>(content))
        {
            // no operation
        }


        public ResultTypeEnum ResultType { get; }

        dynamic ICommandResult.Content => Content;

        public IImmutableList<TItem> Content { get; }
    }
}
