namespace BeaverSoft.Texo.Core.Input
{
    public interface IToken
    {
        TokenTypeEnum Type { get; }

        string Input { get; }

        string Title { get; }
    }
}