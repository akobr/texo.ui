namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public interface ITextumUi
    {
        string Prompt { get; }

        bool ShowWorkingPathAsPrompt { get; }
    }
}