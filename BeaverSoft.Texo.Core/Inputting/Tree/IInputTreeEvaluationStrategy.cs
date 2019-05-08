namespace BeaverSoft.Texo.Core.Inputting.Tree
{
    public interface IInputTreeEvaluationStrategy
    {
        Input Evaluate(ParsedInput parsedInput);
    }
}