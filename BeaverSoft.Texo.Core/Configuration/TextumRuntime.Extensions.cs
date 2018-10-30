namespace BeaverSoft.Texo.Core.Configuration
{
    public static class TextumRuntimeExtensions
    {
        public static bool IsDefaultCommandDefined(this TextumRuntime configuration)
        {
            return !string.IsNullOrWhiteSpace(configuration?.DefaultCommand);
        }
    }
}
