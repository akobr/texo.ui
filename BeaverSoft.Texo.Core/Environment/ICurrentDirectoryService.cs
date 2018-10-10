namespace BeaverSoft.Texo.Core.Environment
{
    public interface ICurrentDirectoryService
    {
        void SetCurrentDirectory(string directoryPath);

        string GetCurrentDirectory();
    }
}
