namespace BeaverSoft.Texo.Core.Model.Text
{
    public interface IBlock : IElement
    {
        IInline Content { get; }

        string ToString(int level);
    }
}