using System;
using System.Xml.Linq;

namespace BeaverSoft.Texo.Commands.NugetManager.Processing
{
    public interface FileIContentLoader<TContent>
    {
        Uri FilePath { get; }

        TContent Content { get; }

        bool IsSuccess { get; }

        void Load(string filePath);
    }

    public interface IXmlContentLoader : FileIContentLoader<XDocument>
    {
        // no member
    }
}
