using System;
using System.Linq;
using System.Xml.Linq;
using Tobe.Collection.Models;

namespace Tobe.Collection.ImportExport {
    public partial class TobeExportHandler {
        public XElement ExportAgents()
        {
            return new XElement("Agents", 
                _agentRepository.Table.OrderBy(x => x.Name)
                .Select(x => ToElement(x)));
        }

        private static XElement ToElement(Agent agent) {
            var agentElement = new XElement("Agent");

            agentElement.Add(new XAttribute("Name", agent.Name));
            AddAttribute(agentElement, "NameSort", agent.NameSort);
            AddAttribute(agentElement, "FileUnder", agent.FileUnder);
        
            return agentElement;
        }
    }
}