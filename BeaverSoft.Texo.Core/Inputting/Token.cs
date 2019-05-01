namespace BeaverSoft.Texo.Core.Inputting
{
    public class Token : IToken
    {
        public Token(TokenTypeEnum type, string title, string input)
        {
            Type = type;
            Title = title;
            Input = input;
        }

        public TokenTypeEnum Type { get; }

        public string Input { get; }

        public string Title { get; }

        public override string ToString()
        {
            return $"{Input} ({Type})";
        }
    }
}
