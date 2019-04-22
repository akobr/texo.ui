using System;

namespace Commands.Clipboard
{
    public interface IClipboardItem : IEquatable<IClipboardItem>, IEquatable<string>
    {
        string Content { get; }

        int Hash { get; }

        string Thumbnail { get; }
    }
}