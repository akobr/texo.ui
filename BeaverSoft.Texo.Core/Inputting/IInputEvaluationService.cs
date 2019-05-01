using BeaverSoft.Texo.Core.Configuration;
using StrongBeaver.Core;
using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Inputting
{
    public interface IInputEvaluationService : IInitialisable
    {
        Input Evaluate(string input);
    }
}
