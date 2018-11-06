using System;
using System.IO;
using System.Xml.Linq;
using BeaverSoft.Texo.Commands.NugetManager.Projects;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Commands.NugetManager.Processing.Strategies
{
    public abstract class BaseXmlFileProcessiongStrategy : IProjectProcessingStrategy
    {
        private readonly ILogService logger;

        public BaseXmlFileProcessiongStrategy(ILogService logger)
        {
            this.logger = logger;
        }

        public IProject Process(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            XDocument document = TryLoadDocument(filePath);

            if (document == null)
            {
                return null;
            }

            return ProcessXml(document, filePath);
        }

        protected abstract IProject ProcessXml(XDocument document, string filePath);

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
                    logger.Error("Xml file can't be loaded.", exception);
                    return null;
                }
            }
        }
    }
}
