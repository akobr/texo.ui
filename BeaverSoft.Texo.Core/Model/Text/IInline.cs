namespace BeaverSoft.Texo.Core.Model.Text
{
    public interface IInline : IElement
    {
        IInline Content { get; }
    }
}