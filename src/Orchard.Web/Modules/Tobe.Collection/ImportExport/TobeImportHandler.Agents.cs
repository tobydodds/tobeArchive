using System;
using System.Linq;
using System.Xml.Linq;
using Tobe.Collection.Models;
using Orchard.ContentManagement;

namespace Tobe.Collection.ImportExport {
    public partial class TobeImportHandler {
        private void ImportAgents(XElement root) {
            var agentsElement = root.Element("Agents");

            if (agentsElement != null) {
                var agents = _agentRepository.Table.ToArray();

                foreach (var agentElement in agentsElement.Elements("Agent")) {
                    var agentName = agentElement.Attr<string>("Name");
                    var nameSort = agentElement.Attr<string>("NameSort");
                    var fileUnder = agentElement.Attr<string>("FileUnder");
                    var agent = agents.FirstOrDefault(x => x.Name == agentName);

                    if (agent == null) {
                        agent = new Agent {
                            Name = agentName,
                            NameSort = nameSort,
                            FileUnder = fileUnder
                        };
                        _agentRepository.Create(agent);
                    }
                }
            }
        }
    }
}
