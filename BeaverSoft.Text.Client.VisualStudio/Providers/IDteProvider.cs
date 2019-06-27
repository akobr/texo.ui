using EnvDTE80;

namespace BeaverSoft.Text.Client.VisualStudio.Providers
{
    public interface IDteProvider
    {
        DTE2 Get();
    }
}
