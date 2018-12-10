using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public abstract class BlockCollection : IBlockCollection
    {
        protected BlockCollection()
        {
            Children = ImmutableList<IBlock>.Empty;
        }

        protected BlockCollection(IBlock content)
        {
            Children = ImmutableList<IBlock>.Empty.Add(content);
        }

        protected BlockCollection(params IBlock[] content)
        {
            Children = ImmutableList<IBlock>.Empty.AddRange(content);
        }

        protected BlockCollection(ImmutableList<IBlock> children)
        {
            this.Children = children;
        }

        public IInline Content => null;

        protected ImmutableList<IBlock> Children { get; }

        public BlockCollection AddChild(IBlock childToAdd)
        {
            return CreateNewVersion(Children.Add(childToAdd));
        }

        public BlockCollection AddChildren(IEnumerable<IBlock> childrenToAdd)
        {
            return CreateNewVersion(Children.AddRange(childrenToAdd));
        }

        public IEnumerator<IBlock> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            foreach (IBlock block in Children)
            {
                result.Append(block);

                if (result.ToString(result.Length - System.Environment.NewLine.Length,
                        System.Environment.NewLine.Length) != System.Environment.NewLine)
                {
                    result.AppendLine();
                }

                result.AppendLine();
            }

            if (result.Length > 0)
            {
                result.Remove(result.Length - System.Environment.NewLine.Length,
                    System.Environment.NewLine.Length);
            }

            return result.ToString();
        }

        protected abstract BlockCollection CreateNewVersion(ImmutableList<IBlock> children);
    }
}