namespace Commands.CodeBaseSearch
{
    public interface IReporter
    {
        void Report(string progress);

        void Finish();
    }
}