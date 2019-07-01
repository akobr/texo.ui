using EnvDTE80;

namespace BeaverSoft.Text.Client.VisualStudio.Core.Providers
{
    public interface IDteProvider
    {
        DTE2 Get();
    }
}
