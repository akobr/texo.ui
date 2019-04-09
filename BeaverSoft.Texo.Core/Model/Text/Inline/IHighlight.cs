using System.Drawing;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public interface IHighlight : IEmphasis
    {
        Color Color { get; }
    }
}