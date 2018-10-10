﻿namespace BeaverSoft.Texo.Core.Input
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
    }
}
