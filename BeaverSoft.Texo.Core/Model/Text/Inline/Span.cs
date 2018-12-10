using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public class Span : ISpan
    {
        private readonly ImmutableList<IInline> children;

        public Span(IInline content)
        {
            children = ImmutableList<IInline>.Empty.Add(content);
        }

        public Span(params IInline[] content)
        {
            children = ImmutableList<IInline>.Empty.AddRange(content);
        }

        private Span(ImmutableList<IInline> children)
        {
            this.children = children;
        }

        public IInline Content => null;


        public IEnumerator<IInline> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return children.GetEnumerator();
        }

        public IImmutableList<IInline> Children => children;

        public Span AddChild(IInline childToAdd)
        {
            return new Span(children.Add(childToAdd));
        }

        public Span AddChildren(IEnumerable<IInline> childrenToAdd)
        {
            return new Span(children.AddRange(childrenToAdd));
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            foreach (IInline child in children)
            {
                result.Append(child);
            }

            return result.ToString();
        }
    }
}