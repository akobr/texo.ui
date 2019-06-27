using EnvDTE80;

namespace BeaverSoft.Text.Client.VisualStudio.Providers
{
    public class DteProvider : IDteProvider
    {
        private readonly DTE2 dte;

        public DteProvider(DTE2 dte)
        {
            this.dte = dte;
        }

        public DTE2 Get()
        {
            return dte;
        }
    }
}
