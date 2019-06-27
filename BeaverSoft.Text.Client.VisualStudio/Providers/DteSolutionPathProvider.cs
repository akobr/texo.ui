namespace BeaverSoft.Text.Client.VisualStudio.Providers
{
    public class DteSolutionPathProvider : ISolutionPathProvider
    {
        private readonly IDteProvider dteProvider;

        public DteSolutionPathProvider(IDteProvider dteProvider)
        {
            this.dteProvider = dteProvider;
        }

        public string GetPath()
        {
            return dteProvider.Get().Solution?.FileName;
        }
    }
}
