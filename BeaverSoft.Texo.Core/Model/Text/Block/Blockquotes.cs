using System.Collections.Generic;
using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public class Blockquotes : BlockCollection, IBlockquotes
    {
        public Blockquotes()
            : base()
        {
            // no operation
        }

        public Blockquotes(IBlock content)
            : base(content)
        {
            // no operation
        }

        public Blockquotes(params IBlock[] content)
            : base(content)
        {
            // no operation
        }

        private Blockquotes(ImmutableList<IBlock> children)
            : base(children)
        {
            // no operation
        }

        public new Blockquotes AddChild(IBlock childToAdd)
        {
            return new Blockquotes(Children.Add(childToAdd));
        }

        public new Blockquotes AddChildren(IEnumerable<IBlock> childrenToAdd)
        {
            return new Blockquotes(Children.AddRange(childrenToAdd));
        }

        protected override BlockCollection CreateNewVersion(ImmutableList<IBlock> children)
        {
            return new Blockquotes(children);
        }
    }
}