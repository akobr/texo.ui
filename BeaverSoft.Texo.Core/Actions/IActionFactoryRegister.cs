namespace BeaverSoft.Texo.Core.Actions
{
    public interface IActionFactoryRegister
    {
        void RegisterFactory(IActionFactory factory, params string[] actionNames);

        void UnregisterFactory(params string[] actionNames);
    }
}
