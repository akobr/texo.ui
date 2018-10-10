using BeaverSoft.Texo.Core.Input;

namespace BeaverSoft.Texo.Core.InputTree
{
    public interface IInputTreeEvaluationStrategy
    {
        IInput Evaluate(IParsedInput parsedInput);
    }
}