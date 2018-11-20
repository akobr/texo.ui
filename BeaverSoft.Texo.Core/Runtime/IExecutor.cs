namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IExecutor
    {
        Input.Input PreProcess(string input, int cursorPosition);

        void Process(string input);
    }
}