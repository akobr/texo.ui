using BeaverSoft.Texo.Core.Input;

namespace BeaverSoft.Texo.Core.Services
{
    public interface IInputParseService
    {
        IParsedInput Parse(string input);
    }
}