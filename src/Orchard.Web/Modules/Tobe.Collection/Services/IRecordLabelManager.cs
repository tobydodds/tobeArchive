using System.Collections.Generic;
using System.Linq;
using Tobe.Collection.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Tobe.Collection.Services {
    public interface IRecordLabelManager : IDependency {
        IQueryable<RecordLabel> Query();
        void RemoveRecordLabelsFromContent(int contentItemId, string fieldName = null, IEnumerable<int> except = null);
        ContentRecordLabels AddRecordLabelToContent(ContentItem contentItem, string fieldName, RecordLabel recordLabel);
        IEnumerable<ContentItem> GetContentItemsByRecordLabel(int recordLabelId, VersionOptions versionOptions = null);
        IEnumerable<ContentItem> GetContentItemsByRecordLabels(IEnumerable<int> recordLabelIds, string contentType = null, VersionOptions versionOptions = null);
        IEnumerable<RecordLabel> GetRecordLabelsByContent(int contentItemId, string fieldName);
    }
}