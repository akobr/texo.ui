namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public interface ITextumConfiguration
    {
        ITextumRuntime Runtime { get; }

        ITextumEnvironment Environment { get; }

        ITextumUi Ui { get; }
    }
}