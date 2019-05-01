using BeaverSoft.Texo.Core.Inputting;
using Xunit;

namespace BeaverSoft.Texo.Test.Core
{
    public class InputParsingTests
    {
        [Fact]
        public void InputParsing_SimpleCommand_TwoTokens()
        {
            const string input = "cd ../folder";
            InputParseService parser = new InputParseService();
            ParsedInput parsed = parser.Parse(input);

            Assert.Equal(input, parsed.RawInput);
            Assert.Equal(2, parsed.Tokens.Count);
            Assert.Equal("cd", parsed.Tokens[0]);
            Assert.Equal("../folder", parsed.Tokens[1]);
        }

        [Fact]
        public void InputParsing_CommandWithParametrisedOption_FourTokens()
        {
            const string input = "manager --ignore-folders ../folder ../second/sub";
            InputParseService parser = new InputParseService();
            ParsedInput parsed = parser.Parse(input);

            Assert.Equal(input, parsed.RawInput);
            Assert.Equal(4, parsed.Tokens.Count);
            Assert.Equal("manager", parsed.Tokens[0]);
            Assert.Equal("--ignore-folders", parsed.Tokens[1]);
            Assert.Equal("../folder", parsed.Tokens[2]);
            Assert.Equal("../second/sub", parsed.Tokens[3]);
        }

        [Fact]
        public void InputParsing_CommandWithTwoComplexParameters_ThreeTokens()
        {
            const string input = "_command42 \"complex param '## $ ! | text' \" 'second complex param with \"inner text\"'";
            InputParseService parser = new InputParseService();
            ParsedInput parsed = parser.Parse(input);

            Assert.Equal(input, parsed.RawInput);
            Assert.Equal(3, parsed.Tokens.Count);
            Assert.Equal("_command42", parsed.Tokens[0]);
            Assert.Equal("\"complex param '## $ ! | text' \"", parsed.Tokens[1]);
            Assert.Equal("'second complex param with \"inner text\"'", parsed.Tokens[2]);
        }
    }
}
