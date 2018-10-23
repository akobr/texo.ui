namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IExecutor
    {
        void PreProcess(string input);

        void Process(string input);
    }
}