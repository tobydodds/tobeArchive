using System;
using System.Linq;
using Tobe.Collection.Fields;
using Tobe.Collection.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.UI.Resources;

namespace Tobe.Collection.Handlers {
    public class RecordLabelFieldHandler : ContentHandler {
        private readonly IRecordLabelManager _recordLabelManager;
        private readonly IResourceManager _resourceManager;

        public RecordLabelFieldHandler(IRecordLabelManager recordLabelManager, IResourceManager resourceManager) {
            _recordLabelManager = recordLabelManager;
            _resourceManager = resourceManager;
        }

        protected override void BuildDisplayShape(BuildDisplayContext context) {
            if (context.DisplayType != "Detail")
                return;

            var recordLabelsFields = context.ContentItem.Parts.SelectMany(part => part.Fields.Where(field => field.FieldDefinition.Name == "RecordLabelsField")).Cast<RecordLabelsField>();
            var recordLabels = recordLabelsFields.SelectMany(x => _recordLabelManager.GetRecordLabelsByContent(context.ContentItem.Id, x.Name)).Select(x => x.Name).Distinct().ToArray();

            if (!recordLabels.Any())
                return;

            var existingMeta = _resourceManager.GetRegisteredMetas().FirstOrDefault(x => x.Name == "RecordLabel");

            if (existingMeta != null) {
                var recordLabelNames = existingMeta.Content.Split('|').ToList();
                recordLabelNames.AddRange(recordLabels);
                recordLabels = recordLabelNames.Distinct().ToArray();
            }

            _resourceManager.AppendMeta(new MetaEntry {
                Name = "RecordLabel",
                Content = String.Join("|", recordLabels)
            }, "|");
        }

        protected override void Initialized(InitializingContentContext context) {
            var recordLabelsFields = context.ContentItem.Parts.SelectMany(x => x.Fields).Where(x => x.FieldDefinition.Name == typeof (RecordLabelsField).Name).Select(x => (RecordLabelsField)x);

            foreach (var field in recordLabelsFields) {
                var closureField = field;
                field._recordLabelsField.Loader(() => _recordLabelManager.GetRecordLabelsByContent(context.ContentItem.Id, closureField.Name).ToArray());
            }
        }
    }
}