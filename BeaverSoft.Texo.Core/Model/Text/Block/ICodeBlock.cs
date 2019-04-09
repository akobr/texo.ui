namespace BeaverSoft.Texo.Core.Model.Text
{
    public interface ICodeBlock : IBlock
    {
        string Language { get; }
    }
}