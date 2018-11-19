namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IExecutor
    {
        Input.Input PreProcess(string input);

        void Process(string input);
    }
}