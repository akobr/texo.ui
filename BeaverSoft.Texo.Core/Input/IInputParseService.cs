namespace BeaverSoft.Texo.Core.Input
{
    public interface IInputParseService
    {
        IParsedInput Parse(string input);
    }
}