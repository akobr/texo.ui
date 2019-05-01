namespace BeaverSoft.Texo.Core.Inputting.InputTree
{
    public interface IInputTreeEvaluationStrategy
    {
        Input Evaluate(ParsedInput parsedInput);
    }
}