using System;
using System.Linq;
using System.Xml;
using Tobe.Collection.Helpers;
using Tobe.Collection.Models;
using Tobe.Collection.Services;
using Tobe.Collection.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using System.Xml.Linq;

namespace Tobe.Collection.Drivers {
    public class AlbumPartDriver : ContentPartDriver<AlbumPart> {
        private readonly IAlbumManager _albumManager;
        private readonly IArtistsManager _artistsManager;
        private readonly ICreditsManager _creditsManager;

        public AlbumPartDriver(
            IAlbumManager albumManager, 
            IArtistsManager artistsManager,
            ICreditsManager creditsManager) {

            _albumManager = albumManager;
            _artistsManager = artistsManager;
            _creditsManager = creditsManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override DriverResult Display(AlbumPart part, string displayType, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_Album", () => shapeHelper.Parts_Album()),
                ContentShape("Parts_Album_Cover", () => shapeHelper.Parts_Album_Cover()),
                ContentShape("Parts_Album_Description", () => shapeHelper.Parts_Album_Description()),
                ContentShape("Parts_Album_Notes", () => shapeHelper.Parts_Album_Notes()),
                ContentShape("Parts_Album_Countries", () => shapeHelper.Parts_Album_Countries()));
        }

        protected override DriverResult Editor(AlbumPart part, dynamic shapeHelper) {
            return Editor(part, null, shapeHelper);
        }

        protected override DriverResult Editor(AlbumPart part, IUpdateModel updater, dynamic shapeHelper) {
            var albumEditor = ContentShape("Parts_Album_Edit", () => {
                var viewModel = new AlbumViewModel {
                    AmgRatingId = part.Record.AmgRating != null ? part.Record.AmgRating.Id : default(int?),
                    FormatId = part.Record.Format != null ? part.Record.Format.Id : default(int?),
                    LengthId = part.Record.Length != null ? part.Record.Length.Id : default(int?),
                    ArtistDisplayName = part.ArtistDisplayName,
                    ArtistNameSort = part.ArtistNameSort,
                    CatalogNumber = part.CatalogNumber,
                    LocalNumber = part.LocalNumber,
                    Description = part.Description,
                    Notes = part.Notes,
                    PurchasedAt = part.PurchasedAt,
                    RecordedAt = part.RecordedAt,
                    OriginalReleaseDate = part.OriginalReleaseDate,
                    PurchaseDate = part.PurchaseDate,
                    VersionReleaseDate = part.VersionReleaseDate,
                    ColoredVinyl = part.ColoredVinyl,
                    CoverArtPick = part.CoverArtPick,
                    OriginalPressing = part.OriginalPressing,
                    TobyPick = part.TobyPick,
                    PricePaid = part.PricePaid,
                    AvailableAmgRatings = _albumManager.GetAmgRatings().ToList(),
                    AvailableFormats = _albumManager.GetFormats().ToList(),
                    AvailableLengths = _albumManager.GetLengths().ToList()
                };

                if (updater != null) {
                    if (updater.TryUpdateModel(viewModel, Prefix, null, new[] { "AllAgents", "AllArtists", "AllDelimiters", "AllRoles" })) {
                            part.AmgRating = viewModel.AmgRatingId != null ? _albumManager.GetAmgRating(viewModel.AmgRatingId.Value) : default(AmgRating);
                            part.Format = viewModel.FormatId != null ? _albumManager.GetFormat(viewModel.FormatId.Value) : default(Format);
                            part.Length = viewModel.LengthId != null ? _albumManager.GetLength(viewModel.LengthId.Value) : default(Length);
                            part.ArtistDisplayName = viewModel.ArtistDisplayName;
                            part.ArtistNameSort = viewModel.ArtistNameSort;
                            part.CatalogNumber = viewModel.CatalogNumber;
                            part.LocalNumber = viewModel.LocalNumber;
                            part.Description = viewModel.Description;
                            part.Notes = viewModel.Notes;
                            part.OriginalReleaseDate = viewModel.OriginalReleaseDate;
                            part.PurchasedAt = viewModel.PurchasedAt;
                            part.RecordedAt = viewModel.RecordedAt;
                            part.PurchaseDate = viewModel.PurchaseDate;
                            part.VersionReleaseDate = viewModel.VersionReleaseDate;
                            part.ColoredVinyl = viewModel.ColoredVinyl;
                            part.CoverArtPick = viewModel.CoverArtPick;
                            part.OriginalPressing = viewModel.OriginalPressing;
                            part.TobyPick = viewModel.TobyPick;
                            part.PricePaid = viewModel.PricePaid;                        
                        
                        }
                    }

                return shapeHelper.EditorTemplate(TemplateName: "Parts/Album", Model: viewModel, Prefix: Prefix);
            });
            return Combined(albumEditor);        
        }



        protected override void Exporting(AlbumPart part, ExportContentContext context) {  

            context.Element(part.PartDefinition.Name).SetAttributeValue("ArtistDisplayName", part.ArtistDisplayName);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CatalogNumber", part.CatalogNumber);
            context.Element(part.PartDefinition.Name).SetAttributeValue("LocalNumber", part.LocalNumber);
            context.Element(part.PartDefinition.Name).SetAttributeValue("OriginalReleaseDate", part.OriginalReleaseDate);
            context.Element(part.PartDefinition.Name).SetAttributeValue("PurchasedAt", part.PurchasedAt);
            context.Element(part.PartDefinition.Name).SetAttributeValue("PurchaseDate", part.PurchaseDate);
            context.Element(part.PartDefinition.Name).SetAttributeValue("RecordedAt", part.RecordedAt);
            context.Element(part.PartDefinition.Name).SetAttributeValue("VersionReleaseDate", part.VersionReleaseDate);
            context.Element(part.PartDefinition.Name).SetAttributeValue("ColoredVinyl", part.ColoredVinyl);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CoverArtPick", part.CoverArtPick);
            context.Element(part.PartDefinition.Name).SetAttributeValue("OriginalPressing", part.OriginalPressing);
            context.Element(part.PartDefinition.Name).SetAttributeValue("TobyPick", part.TobyPick);
            context.Element(part.PartDefinition.Name).SetAttributeValue("PricePaid", part.PricePaid);

            if (part.AmgRating != null)
                context.Element(part.PartDefinition.Name).SetAttributeValue("AmgRatingName", part.AmgRating.Name);

            if (part.Format != null)
                context.Element(part.PartDefinition.Name).SetAttributeValue("FormatName", part.Format.Name);

            if (part.Length != null)
                context.Element(part.PartDefinition.Name).SetAttributeValue("LengthName", part.Length.Name);

            if (!String.IsNullOrWhiteSpace(part.Description)) {
                context.Element(part.PartDefinition.Name).Add(new XElement("Description", part.Description));
            }

            if (!String.IsNullOrWhiteSpace(part.Notes)) {
                context.Element(part.PartDefinition.Name).Add(new XElement("Notes", part.Notes));
            }
        }

        protected override void Importing(AlbumPart part, ImportContentContext context) {      
            context.ImportAttribute(part.PartDefinition.Name, "AmgRatingName", value => part.AmgRating = _albumManager.GetAmgRatingByName(value));
            context.ImportAttribute(part.PartDefinition.Name, "FormatName", value => part.Format = _albumManager.GetFormatByName(value));
            context.ImportAttribute(part.PartDefinition.Name, "LengthName", value => part.Length = _albumManager.GetLengthByName(value));
            context.ImportAttribute(part.PartDefinition.Name, "ArtistDisplayName", value => part.ArtistDisplayName = value);
            context.ImportAttribute(part.PartDefinition.Name, "CatalogNumber", value => part.CatalogNumber = value);
            context.ImportAttribute(part.PartDefinition.Name, "LocalNumber", value => part.LocalNumber = value);
            context.ImportAttribute(part.PartDefinition.Name, "OriginalReleaseDate", value => part.OriginalReleaseDate = value);
            context.ImportAttribute(part.PartDefinition.Name, "PurchasedAt", value => part.PurchasedAt = value);
            context.ImportAttribute(part.PartDefinition.Name, "PurchaseDate", value => part.PurchaseDate = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Utc));
            context.ImportAttribute(part.PartDefinition.Name, "RecordedAt", value => part.RecordedAt = value);
            context.ImportAttribute(part.PartDefinition.Name, "VersionReleaseDate", value => part.VersionReleaseDate = value);
            context.ImportAttribute(part.PartDefinition.Name, "ColoredVinyl", value => part.ColoredVinyl = XmlConvert.ToBoolean(value));
            context.ImportAttribute(part.PartDefinition.Name, "CoverArtPick", value => part.CoverArtPick = XmlConvert.ToBoolean(value));
            context.ImportAttribute(part.PartDefinition.Name, "OriginalPressing", value => part.OriginalPressing = XmlConvert.ToBoolean(value));
            context.ImportAttribute(part.PartDefinition.Name, "TobyPick", value => part.TobyPick = XmlConvert.ToBoolean(value));
            context.ImportAttribute(part.PartDefinition.Name, "PricePaid", value => part.PricePaid = XmlConvert.ToDecimal(value));

            var descriptionElement = context.Data.Element(part.PartDefinition.Name).Element("Description");

            if (descriptionElement != null) {
                part.Description = descriptionElement.Value;
            }

            var notesElement = context.Data.Element(part.PartDefinition.Name).Element("Notes");

            if (notesElement != null) {
                part.Notes = notesElement.Value;
            }

        }

        private bool ValidArtistName(IUpdateModel updater, string agentName, out Agent agent) {
            agent = null;
            if (String.IsNullOrWhiteSpace(agentName))
                return true;

            agent = _artistsManager.GetAgentByName(agentName.Trim());

            if (agent == null) {
                updater.AddModelError("AgentName", T("The specified agent name does not exist."));
                return false;
            }

            return true;
        }

        private bool ValidAgentName(IUpdateModel updater, string agentName, out Agent agent) {
            agent = null;
            if (String.IsNullOrWhiteSpace(agentName))
                return true;

            agent = _creditsManager.GetAgentByName(agentName.Trim());

            if (agent == null) {
                updater.AddModelError("AgentName", T("The specified agent name does not exist."));
                return false;
            }

            return true;
        }
    }
}