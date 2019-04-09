using System.Collections.Generic;
using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public class Section : BlockCollection, ISection
    {
        public Section()
            : base()
        {
            // no operation
        }

        public Section(IBlock content)
            : base(content)
        {
            // no operation
        }

        public Section(params IBlock[] content)
            : base(content)
        {
            // no operation
        }

        private Section(ImmutableList<IBlock> children)
            : base(children)
        {
            // no operation
        }

        public new Section AddChild(IBlock childToAdd)
        {
            return new Section(Children.Add(childToAdd));
        }

        public new Section AddChildren(IEnumerable<IBlock> childrenToAdd)
        {
            return new Section(Children.AddRange(childrenToAdd));
        }

        protected override BlockCollection CreateNewVersion(ImmutableList<IBlock> children)
        {
            return new Section(children);
        }
    }

}
