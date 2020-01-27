using BeaverSoft.Texo.Core.Intellisense;
using Commands.Dotnet;
using Commands.Git;
using StrongBeaver.Core.Container;

namespace BeaverSoft.Text.Client.VisualStudio.Startup
{
    public static class IntellisenseConfig
    {
        public static void RegisterIntellisense(this SimpleIoc container)
        {
            IIntellisenseService service = container.GetInstance<IIntellisenseService>();

            container.Register<DotnetIntellisenseProvider>();
            service.RegisterExternalHelpProvider("dotnet", container.GetInstance<DotnetIntellisenseProvider>());

            container.Register<GitIntellisenseProvider>();
            service.RegisterExternalHelpProvider("git", container.GetInstance<GitIntellisenseProvider>());
        }
    }
}
