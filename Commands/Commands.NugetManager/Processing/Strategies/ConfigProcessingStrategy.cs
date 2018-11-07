using System;
using System.Collections.Immutable;
using System.Xml.Linq;
using BeaverSoft.Texo.Commands.NugetManager.Model.Configs;
using BeaverSoft.Texo.Commands.NugetManager.Model.Sources;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Commands.NugetManager.Processing.Strategies
{
    public class ConfigProcessingStrategy : IConfigProcessingStrategy
    {
        private readonly ILogService logger;

        public ConfigProcessingStrategy(ILogService logger)
        {
            this.logger = logger;
        }

        public IConfig Process(string filePath)
        {
            IXmlContentLoader loader = new XmlContentLoader(logger);
            loader.Load(filePath);

            if (!loader.IsSuccess)
            {
                return null;
            }

            return ProcessConfig(loader.Content, loader.FilePath);
        }

        private IConfig ProcessConfig(XDocument document, Uri filePath)
        {
            XElement root = document.Root;

            if (root == null || root.Name.LocalName != "configuration")
            {
                return null;
            }

            XNamespace xmlNamespace = root.GetDefaultNamespace();
            var sources = ImmutableList<ISourceInfo>.Empty.ToBuilder();

            foreach (XElement elementSources in root.Descendants(xmlNamespace + "packageSources"))
            {
                foreach (XElement elementAdd in elementSources.Elements(xmlNamespace + "add"))
                {
                    string value = (string)elementAdd.Attribute("value");

                    if (Uri.TryCreate(value, UriKind.Absolute, out Uri sourceUrl))
                    {
                        sources.Add(new SourceInfo(sourceUrl));
                    }
                    else
                    {
                        logger.Warn("Invalid nuget source address.", value);
                    }
                }
            }

            return new Config(filePath, sources.ToImmutable());
        }
    }
}
