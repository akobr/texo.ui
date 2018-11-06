using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using BeaverSoft.Texo.Commands.NugetManager.Projects;

namespace BeaverSoft.Texo.Commands.NugetManager.Processing.Strategies
{
    public class ConfigProcessingStrategy : BaseXmlFileProcessiongStrategy
    {
        protected override IProject ProcessXml(XDocument document, string filePath)
        {
            XElement root = document.Root;

            if (root == null || root.Name.LocalName != "configuration")
            {
                return null;
            }

            XNamespace xmlNamespace = root.GetDefaultNamespace();

            foreach (XElement elementSources in root.Descendants(xmlNamespace + "packageSources"))
            {
                foreach (XElement elementAdd in elementSources.Elements(xmlNamespace + "add"))
                {

                }
            }
        }
    }
}
