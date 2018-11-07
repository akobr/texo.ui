using System;
using System.IO;
using System.Xml.Linq;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Commands.NugetManager.Processing
{
    public class XmlContentLoader : IXmlContentLoader
    {
        private readonly ILogService logger;

        public XmlContentLoader(ILogService logger)
        {
            this.logger = logger;
        }

        public Uri FilePath { get; private set; }

        public XDocument Content { get; private set; }

        public bool IsSuccess { get; private set; }

        public void Load(string filePath)
        {
            Reset();

            if (!File.Exists(filePath))
            {
                return;
            }

            string fullPath = Path.GetFullPath(filePath);
            FilePath = new Uri(fullPath, UriKind.Absolute);
            Content =  TryLoadDocument(filePath);
        }

        public void Reset()
        {
            FilePath = null;
            Content = null;
            IsSuccess = false;
        }

        private XDocument TryLoadDocument(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    return XDocument.Load(fileStream);
                }
                catch (Exception exception)
                {
                    logger.Error("Xml file haven't been loaded.", exception);
                    return null;
                }
            }
        }
    }
}
