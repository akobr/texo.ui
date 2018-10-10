namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public interface IParameter
    {
        string Key { get; }

        bool IsOptional { get; }

        bool IsRepeatable { get; }

        string ArgumentTemplate { get; }

        IDocumentation Documentation { get; }
    }
}