using System;
using System.Collections.Generic;
using System.Linq;
using Tobe.Collection.Models;
using Orchard.ContentManagement;
using Orchard.Data;

namespace Tobe.Collection.Services {
    public class RecordLabelManager : IRecordLabelManager {
        private readonly IRepository<RecordLabel> _recordLabelRepository;
        private readonly IRepository<ContentRecordLabels> _contentRecordLabelsRepository;
        private readonly IContentManager _contentManager;

        public RecordLabelManager(
            IRepository<RecordLabel> recordLabelRepository, 
            IRepository<ContentRecordLabels> contentRecordLabelsRepository, 
            IContentManager contentManager) {

            _recordLabelRepository = recordLabelRepository;
            _contentRecordLabelsRepository = contentRecordLabelsRepository;
            _contentManager = contentManager;
        }

        public IQueryable<RecordLabel> Query() {
            return _recordLabelRepository.Table;
        }

        public void RemoveRecordLabelsFromContent(int contentItemId, string fieldName = null, IEnumerable<int> except = null) {
            var recordLabelsQuery = _contentRecordLabelsRepository.Table.Where(x => x.ContentItem.Id == contentItemId && x.FieldName == fieldName);

            if (except != null) {
                recordLabelsQuery = recordLabelsQuery.Where(x => !except.Contains(x.Id));
            }

            foreach (var recordLabel in recordLabelsQuery) {
                _contentRecordLabelsRepository.Delete(recordLabel);
            }
        }

        public ContentRecordLabels AddRecordLabelToContent(ContentItem contentItem, string fieldName, RecordLabel recordLabel) {
            var contentRecordLabels = new ContentRecordLabels {
                ContentItem = contentItem.Record,
                FieldName = fieldName,
                RecordLabel = recordLabel
            };

            _contentRecordLabelsRepository.Create(contentRecordLabels);
            return contentRecordLabels;
        }

        public IEnumerable<ContentItem> GetContentItemsByRecordLabel(int recordLabelId, VersionOptions versionOptions = null) {
            var contentItemIds = _contentRecordLabelsRepository.Table.Where(x => x.RecordLabel.Id == recordLabelId).Select(x => x.ContentItem.Id).Distinct();
            return _contentManager.GetMany<ContentItem>(contentItemIds, versionOptions ?? VersionOptions.Latest, QueryHints.Empty);
        }

        public IEnumerable<ContentItem> GetContentItemsByRecordLabels(IEnumerable<int> recordLabelIds, string contentType = null, VersionOptions versionOptions = null) {
            var query = 
                from contentRecordLabel in _contentRecordLabelsRepository.Table
                where recordLabelIds.Contains(contentRecordLabel.RecordLabel.Id) 
                select contentRecordLabel;

            if (!String.IsNullOrWhiteSpace(contentType)) {
                query =
                    from contentRecordLabel in query 
                    where contentRecordLabel.ContentItem.ContentType.Name == contentType 
                    select contentRecordLabel;
            }

            var contentItemIds = query.Select(x => x.ContentItem.Id).Distinct();
            return _contentManager.GetMany<ContentItem>(contentItemIds, versionOptions ?? VersionOptions.Latest, QueryHints.Empty);
        }

        public IEnumerable<RecordLabel> GetRecordLabelsByContent(int contentItemId, string fieldName) {
            return _contentRecordLabelsRepository.Table.Where(x => x.ContentItem.Id == contentItemId && x.FieldName == fieldName).Select(x => x.RecordLabel).Distinct();
        }
    }
}