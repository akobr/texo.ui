using System;
using System.Text;

namespace Commands.Clipboard
{
    public class ClipboardItem : IClipboardItem
    {
        private string thumbnail;

        public ClipboardItem(string content)
        {
            Content = content;

            if (string.IsNullOrWhiteSpace(content))
            {
                Hash = 0;
                Content = string.Empty;
                thumbnail = string.Empty;
                return;
            }

            Hash = content.GetHashCode();
        }

        public int Hash { get; }

        public string Thumbnail
        {
            get
            {
                if (thumbnail == null)
                {
                    thumbnail = CalculateThumbnail(Content);
                }

                return thumbnail;
            }
        }

        public string Content { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (obj is string hash)
            {
                return Equals(hash);
            }

            return Equals(obj as IClipboardItem);
        }

        public bool Equals(IClipboardItem other)
        {
            return other != null && Hash == other.Hash;
        }

        public bool Equals(string hash)
        {
            return hash != null && Hash == hash.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Hash;
        }

        private static string CalculateThumbnail(string content)
        {
            string[] lines = content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder builder = new StringBuilder();
            int linesCount = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (builder.Length + line.Length > 256)
                {
                    int lineLength = 256 - builder.Length;

                    if (lineLength >= 3)
                    {
                        builder.Append(line.Substring(0, lineLength));
                    }

                    builder.Append("...");
                    break;
                }

                builder.Append(line);
                linesCount++;

                if (i + 1 < lines.Length)
                {
                    builder.AppendLine();

                    if (linesCount > 3)
                    {
                        builder.Append("...");
                        break;
                    }
                }
            }

            return builder.ToString();
        }
    }
}
