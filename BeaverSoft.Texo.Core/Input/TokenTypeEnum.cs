namespace BeaverSoft.Texo.Core.Input
{
    public enum TokenTypeEnum
    {
        Wrong = -1,
        Unknown = 0,
        Query,
        Option,
        OptionList,
        Parameter,
        EndOfParameterList,
    }
}