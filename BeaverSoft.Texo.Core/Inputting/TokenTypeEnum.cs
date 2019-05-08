namespace BeaverSoft.Texo.Core.Inputting
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