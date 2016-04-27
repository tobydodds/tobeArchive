using System.Linq;
using System.Xml.Linq;
using Tobe.Collection.Fields;
using Tobe.Collection.Models;
using Tobe.Collection.Services;
using Tobe.Collection.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace Tobe.Collection.Drivers {
    public class RecordLabelsFieldDriver : ContentFieldDriver<RecordLabelsField> {
        private readonly IRecordLabelManager _recordLabelManager;

        public RecordLabelsFieldDriver(IRecordLabelManager recordLabelManager) {
            _recordLabelManager = recordLabelManager;
        }

        protected override DriverResult Display(ContentPart part, RecordLabelsField field, string displayType, dynamic shapeHelper) {
            return ContentShape("Fields_RecordLabels", GetDifferentiator(field), () => shapeHelper.Fields_RecordLabels(RecordLabels: _recordLabelManager.GetRecordLabelsByContent(part.Id, field.Name).ToArray()));
        }

        protected override DriverResult Editor(ContentPart part, RecordLabelsField field, dynamic shapeHelper) {
            return Editor(part, field, null, shapeHelper);
        }

        protected override DriverResult Editor(ContentPart part, RecordLabelsField field, IUpdateModel updater, dynamic shapeHelper) {
            return ContentShape("Fields_RecordLabels_Edit", GetDifferentiator(field), () => {
                var prefix = GetPrefix(field, part);
                var model = new RecordLabelsViewModel {
                    AllRecordLabels = _recordLabelManager.Query().ToArray(),
                    ContentField = field
                };

                if (updater != null) {
                    if (updater.TryUpdateModel(model, prefix, null, null)) {
                        UpdateRecordLabels(part, field, model);
                    }
                }

                var recordLabels = _recordLabelManager.GetRecordLabelsByContent(part.Id, field.Name);
                model.RecordLabels = recordLabels.Select(x => new RecordLabelViewModel {
                    Id = x.Id,
                    Name = x.Name
                }).ToArray();
                return shapeHelper.EditorTemplate(TemplateName: "Fields/RecordLabels", Model: model, Prefix: prefix);
            });
        }

        protected override void Exporting(ContentPart part, RecordLabelsField field, ExportContentContext context) {
            var fieldElement = context.Element(field.FieldDefinition.Name + "." + field.Name);
            var recordLabels = _recordLabelManager.GetRecordLabelsByContent(part.Id, field.Name).ToArray();

            if (recordLabels.Any()) {
                fieldElement.Add(recordLabels.Select(x => new XElement("RecordLabel", new XAttribute("Name", x.Name))));
            }
        }

        protected override void Importing(ContentPart part, RecordLabelsField field, ImportContentContext context) {
            var fieldElement = context.Data.Element(field.FieldDefinition.Name + "." + field.Name);

            if (fieldElement == null)
                return;

            var recordLabelsDictionary = _recordLabelManager.Query().ToDictionary(x => x.Name);

            foreach (var recordLabel in fieldElement.Elements("RecordLabel")
                .Select(recordLabelElement => recordLabelElement.Attr<string>("Name"))
                .Select(recordLabelName => recordLabelsDictionary.ContainsKey(recordLabelName) ? recordLabelsDictionary[recordLabelName] : default(RecordLabel))
                .Where(recordLabel => recordLabel != null)) {
                    _recordLabelManager.AddRecordLabelToContent(part.ContentItem, field.Name, recordLabel);
            }
        }

        private void UpdateRecordLabels(ContentPart part, RecordLabelsField field, RecordLabelsViewModel model) {
            var postedRecordLabelIds = model.RecordLabels != null ? model.RecordLabels.Select(x => x.Id).ToArray() : Enumerable.Empty<int>();
            var postedRecordLabels = _recordLabelManager.Query().Where(x => postedRecordLabelIds.Contains(x.Id)).ToArray();
            var recordLabels = _recordLabelManager.GetRecordLabelsByContent(part.Id, field.Name);

            _recordLabelManager.RemoveRecordLabelsFromContent(part.Id, field.Name, postedRecordLabelIds);

            foreach (var postedRecordLabel in
                    from postedRecordLabel in postedRecordLabels
                    let recordLabel = recordLabels.FirstOrDefault(x => x.Id == postedRecordLabel.Id)
                    where recordLabel == null
                    select postedRecordLabel) {
                _recordLabelManager.AddRecordLabelToContent(part.ContentItem, field.Name, postedRecordLabel);
            }
        }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(ContentField field) {
            return field.Name;
        }
    }
}