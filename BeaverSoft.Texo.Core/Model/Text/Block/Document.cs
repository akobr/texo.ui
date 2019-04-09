using System.Collections.Generic;
using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public class Document : BlockCollection, IDocument
    {
        public Document()
            : base()
        {
            // no operation
        }

        public Document(IBlock content)
            : base(content)
        {
            // no operation
        }

        public Document(params IBlock[] content)
            : base(content)
        {
            // no operation
        }

        private Document(ImmutableList<IBlock> children)
            : base(children)
        {
            // no operation
        }

        public new Document AddChild(IBlock childToAdd)
        {
            return new Document(Children.Add(childToAdd));
        }

        public new Document AddChildren(IEnumerable<IBlock> childrenToAdd)
        {
            return new Document(Children.AddRange(childrenToAdd));
        }

        protected override BlockCollection CreateNewVersion(ImmutableList<IBlock> children)
        {
            return new Document(children);
        }
    }
}