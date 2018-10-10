﻿using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Services;

namespace BeaverSoft.Texo.Core.Runtime
{
    public class RealtimeCoordinatorService : IRealtimeCoordinatorService
    {
        private readonly IInputParseService parser;

        public RealtimeCoordinatorService(
            IInputParseService parser)
        {
            this.parser = parser;
        }

        public void Process(string input)
        {
            IParsedInput parsedInput = parser.Parse(input);
        }
    }
}
