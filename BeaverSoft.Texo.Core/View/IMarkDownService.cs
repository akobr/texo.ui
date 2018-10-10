namespace BeaverSoft.Texo.Core.View
{
    public interface IMarkDownService
    {
        string Normalise(string text);

        string ToHtml(string text);

        string ToPlainText(string text);
    }
}