namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public static class TextumRuntimeExtensions
    {
        public static bool IsDefaultCommandDefined(this TextumRuntime configuration)
        {
            return !string.IsNullOrWhiteSpace(configuration?.DefaultCommand);
        }
    }
}
