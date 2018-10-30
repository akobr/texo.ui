namespace BeaverSoft.Texo.Core.Input.InputTree
{
    public interface IInputTreeEvaluationStrategy
    {
        Core.Input.Input Evaluate(ParsedInput parsedInput);
    }
}