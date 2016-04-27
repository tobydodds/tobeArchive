using System.Linq;
using System.Xml.Linq;
using Tobe.Collection.Helpers;
using Tobe.Collection.Models;
using Tobe.Collection.Services;
using Tobe.Collection.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace Tobe.Collection.Drivers {
    public class CreditsContainerPartDriver : ContentPartDriver<CreditsContainerPart> {
        private readonly ICreditsManager _creditsManager;

        public CreditsContainerPartDriver(ICreditsManager creditsManager) {
            _creditsManager = creditsManager;
        }

        protected override DriverResult Display(CreditsContainerPart part, string displayType, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_Credits", () => shapeHelper.Parts_Credits()));
        }

        protected override DriverResult Editor(CreditsContainerPart part, dynamic shapeHelper) {
            return Editor(part, null, shapeHelper);
        }

        protected override DriverResult Editor(CreditsContainerPart part, IUpdateModel updater, dynamic shapeHelper) {
            return ContentShape("Parts_Credits_Edit", () => {
                var viewModel = new CreditsContainerViewModel() {
                    AllRoles = _creditsManager.GetRoles().OrderBy(x => x.Name).ToList()
                };

                viewModel.Credits = part.Credits.Select(x => new CreditViewModel {
                    Id = x.Id,
                    AgentId = x.Agent.Id,
                    AgentName = x.Agent.Name,
                    RoleId = x.Role.Id,
                    RoleName = x.Role.Name
                }).ToList();

                if (updater != null) {
                    if (updater.TryUpdateModel(viewModel, Prefix, null, new[] { "AllRoles" })) {
                        UpdateCredits(viewModel, part);
                    }
                }

                return shapeHelper.EditorTemplate(TemplateName: "Parts/Credits", Model: viewModel, Prefix: Prefix);
            });
        }

        protected override void Exporting(CreditsContainerPart part, ExportContentContext context) {
            var partElement = context.Element(part.PartDefinition.Name);
            var credits = part.Credits;

            foreach (var credit in credits) {
                var creditElement = new XElement("Credit");

                creditElement.AddAttribute("AgentName", credit.Agent != null ? credit.Agent.Name : default(string));
                creditElement.AddAttribute("RoleName", credit.Role != null ? credit.Role.Name : default(string));

                partElement.Add(creditElement);
            }
        }

        protected override void Importing(CreditsContainerPart part, ImportContentContext context) {
            var partElement = context.Data.Element(part.PartDefinition.Name);

            if (partElement == null)
                return;

            var agentsDictionary = _creditsManager.GetAgents().Distinct(new DistinctAgentComparer()).ToDictionary(x => x.Name);
            var rolesDictionary = _creditsManager.GetRoles().ToDictionary(x => x.Name);

            foreach (var creditElement in partElement.Elements("Credit")) {
                var agent = creditElement.ImportAttribute<string, Agent>("AgentName", x => agentsDictionary.ContainsKey(x) ? agentsDictionary[x] : default(Agent));
                var role = creditElement.ImportAttribute<string, Role>("RoleName", x => rolesDictionary.ContainsKey(x) ? rolesDictionary[x] : default(Role));
                _creditsManager.AddCredit(part, agent, role);
            }
        }

        private void UpdateCredits(CreditsContainerViewModel viewModel, CreditsContainerPart part) {
            var agents = viewModel.Credits != null ? _creditsManager.GetAgents(viewModel.Credits.Select(x => x.AgentId)) : Enumerable.Empty<Agent>();
            var roles = viewModel.Credits != null ? _creditsManager.GetRoles(viewModel.Credits.Select(x => x.RoleId)) : Enumerable.Empty<Role>();
            var postedCredits = viewModel.Credits != null ? viewModel.Credits.Select(x => x.Id).ToArray() : new int[0];

            _creditsManager.ClearCredits(part.Id, postedCredits);
            if (viewModel.Credits == null)
                return;

            foreach (var credit in viewModel.Credits.Where(x => x.Id == 0 || !postedCredits.Contains(x.Id))) {
                var agent = agents.Single(x => x.Id == credit.AgentId);
                var role = roles.Single(x => x.Id == credit.RoleId);
                _creditsManager.AddCredit(part, agent, role);
            }
        }
    }
}