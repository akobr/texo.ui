namespace BeaverSoft.Texo.Core.Model.Text
{
    public interface ILink : IInline
    {
        string Address { get; }
    }
}