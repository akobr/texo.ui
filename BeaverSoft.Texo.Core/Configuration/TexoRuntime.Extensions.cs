namespace BeaverSoft.Texo.Core.Configuration
{
    public static class TexoRuntimeExtensions
    {
        public static bool IsDefaultCommandDefined(this TexoRuntime configuration)
        {
            return !string.IsNullOrWhiteSpace(configuration?.DefaultCommand);
        }
    }
}
