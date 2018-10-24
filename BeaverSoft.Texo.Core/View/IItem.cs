namespace BeaverSoft.Texo.Core.View
{
    public interface IItem
    {
        string Text { get; }

        TextFormatEnum Format { get; }
    }
}