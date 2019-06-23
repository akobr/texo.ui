namespace Commands.SpinSport
{
    public class TestSolutionDirectoryProvider : ISolutionDirectoryProvider
    {
        public string Get()
        {
            return @"c:\Working\Configurator\TestingSolution";
        }
    }
}
