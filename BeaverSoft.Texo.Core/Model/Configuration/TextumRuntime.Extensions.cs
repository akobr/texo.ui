namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public static class TextumRuntimeExtensions
    {
        public static bool IsDefaultCommandDefined(this ITextumRuntime configuration)
        {
            return !string.IsNullOrWhiteSpace(configuration?.DefaultCommand);
        }
    }
}
