using System;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Streaming.Text
{
    public interface IFormattableTextStream : IWritableTextStream
    {
        void SetBoldText();

        void SetItalicText();

        void SetUnderlineText();

        void SetCrossedOutText();

        void SetForegroundTextColor(ConsoleColor color);

        void SetBackgroundTextColor(ConsoleColor color);

        void ResetFormatting();

        Task SetBoldTextAsync();

        Task SetItalicTextAsync();

        Task SetUnderlineTextAsync();

        Task SetCrossedOutTextAsync();

        Task SetForegroundTextColorAsync(ConsoleColor color);

        Task SetBackgroundTextColorAsync(ConsoleColor color);

        Task ResetFormattingAsync();
    }
}
