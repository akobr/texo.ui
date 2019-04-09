namespace BeaverSoft.Texo.Core.Model.Text
{
    public interface IListItem : IInline
    {
        ushort Level { get; }
    }
}