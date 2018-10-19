using StrongBeaver.Core;

namespace BeaverSoft.Texo.Core.Input
{
    public interface IInputEvaluationService : IInitialisable
    {
        IInput Evaluate(string input);
    }
}
