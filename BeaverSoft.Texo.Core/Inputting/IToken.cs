namespace BeaverSoft.Texo.Core.Inputting
{
    public interface IToken
    {
        TokenTypeEnum Type { get; }

        string Input { get; }

        string Title { get; }
    }
}