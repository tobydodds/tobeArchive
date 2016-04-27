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
    public class ArtistsContainerPartDriver : ContentPartDriver<ArtistsContainerPart> {
        private readonly IArtistsManager _artistsManager;

        public ArtistsContainerPartDriver(IArtistsManager artistsManager) {
            _artistsManager = artistsManager;
        }

        protected override DriverResult Display(ArtistsContainerPart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_Artists", () => shapeHelper.Parts_Artists());
        }

        protected override DriverResult Editor(ArtistsContainerPart part, dynamic shapeHelper) {
            return Editor(part, null, shapeHelper);
        }

        protected override DriverResult Editor(ArtistsContainerPart part, IUpdateModel updater, dynamic shapeHelper) {
            return ContentShape("Parts_Artists_Edit", () => {
                var viewModel = new ArtistsContainerViewModel() {
                    AllDelimiters = _artistsManager.GetDelimiters().OrderBy(x => x.Name).ToList()
                };

                viewModel.Artists = part.Artists.Select(x => new ArtistViewModel {
                    Id = x.Id,
                    AgentId = x.Agent.Id,
                    AgentName = x.Agent.Name,
                    DelimiterId = x.Delimiter != null ? x.Delimiter.Id : 0,
                    DelimiterName = x.Delimiter != null ? x.Delimiter.Name : ""
                }).ToList();

                if (updater != null) {
                    if (updater.TryUpdateModel(viewModel, Prefix, null, new[] { "AllDelimiters" })) {
                        UpdateArtists(viewModel, part);
                    }
                }

                return shapeHelper.EditorTemplate(TemplateName: "Parts/Artists", Model: viewModel, Prefix: Prefix);
            });
        }

        protected override void Exporting(ArtistsContainerPart part, ExportContentContext context) {
            var partElement = context.Element(part.PartDefinition.Name);
            var artists = part.Artists;

            foreach (var artist in artists) {
                var artistElement = new XElement("Artist");

                artistElement.AddAttribute("AgentName", artist.Agent != null ? artist.Agent.Name : default(string));
                artistElement.AddAttribute("DelimiterName", artist.Delimiter != null ? artist.Delimiter.Name : default(string));

                partElement.Add(artistElement);
            }
        }

        protected override void Importing(ArtistsContainerPart part, ImportContentContext context) {
            var partElement = context.Data.Element(part.PartDefinition.Name);

            if (partElement == null)
                return;

            var agentsDictionary = _artistsManager.GetAgents().Distinct(new DistinctAgentComparer()).ToDictionary(x => x.Name);
            var delimitersDictionary = _artistsManager.GetDelimiters().ToDictionary(x => x.Name);

            foreach (var artistElement in partElement.Elements("Artist")) {
                var agent = artistElement.ImportAttribute<string, Agent>("AgentName", x => agentsDictionary.ContainsKey(x) ? agentsDictionary[x] : default(Agent));
                var delimiter = artistElement.ImportAttribute<string, Delimiter>("DelimiterName", x => delimitersDictionary.ContainsKey(x) ? delimitersDictionary[x] : default(Delimiter));
                _artistsManager.AddArtist(part, agent, delimiter);
            }
        }

        private void UpdateArtists(ArtistsContainerViewModel viewModel, ArtistsContainerPart part) {
            var agents = viewModel.Artists != null ? _artistsManager.GetAgents(viewModel.Artists.Select(x => x.AgentId)) : Enumerable.Empty<Agent>();
            var delimiters = viewModel.Artists != null ? _artistsManager.GetDelimiters(viewModel.Artists.Where(x => x.DelimiterId != null).Select(x => x.DelimiterId.Value)) : Enumerable.Empty<Delimiter>();

            if (viewModel.Artists == null)
                return;

            foreach (var artistViewModel in viewModel.Artists) {
                if (artistViewModel.Removed) {
                    var artist = artistViewModel.Id > 0 ? _artistsManager.GetArtist(artistViewModel.Id) : default(Artist);

                    _artistsManager.RemoveArtist(artist);
                }
                else {
                    if (artistViewModel.Id == 0) {
                        var agent = agents.Single(x => x.Id == artistViewModel.AgentId);
                        var delimiter = delimiters.SingleOrDefault(x => x.Id == artistViewModel.DelimiterId);
                        _artistsManager.AddArtist(part, agent, delimiter);
                    }
                }
            }
        }
    }
}