namespace BeaverSoft.Texo.Commands.NugetManager.Model
{
    public interface IPackage
    {
        string Id { get; }

        string Version { get; }

        bool? CanBeUpdated { get; }
    }
}
