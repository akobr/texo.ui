using BeaverSoft.Texo.Core.Streaming.Text;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.View
{
    public class TextStreamItem : IItem
    {
        public TextStreamItem(ITextStream stream, Task outerTask)
        {
            Stream = stream;
            OuterTask = outerTask;
        }

        public ITextStream Stream { get; }

        public Task OuterTask { get; }

        public string Text => string.Empty;

        public TextFormatEnum Format => TextFormatEnum.Plain;

        public IImmutableList<ILink> Actions => ImmutableList<ILink>.Empty;
    }
}
