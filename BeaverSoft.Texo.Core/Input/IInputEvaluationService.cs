using BeaverSoft.Texo.Core.Configuration;
using StrongBeaver.Core;
using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Input
{
    public interface IInputEvaluationService : IInitialisable
    {
        Input Evaluate(string input);
    }
}
