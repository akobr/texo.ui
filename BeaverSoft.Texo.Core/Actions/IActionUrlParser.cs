namespace BeaverSoft.Texo.Core.Actions
{
    public interface IActionUrlParser
    {
        IActionContext Parse(string actionUrl);
    }
}