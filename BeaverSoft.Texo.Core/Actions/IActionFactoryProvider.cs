namespace BeaverSoft.Texo.Core.Actions
{
    public interface IActionFactoryProvider
    {
        IActionFactory GetActionFactory(string actionName);
    }
}