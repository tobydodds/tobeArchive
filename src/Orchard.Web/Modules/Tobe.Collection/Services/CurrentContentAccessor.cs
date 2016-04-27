using System;
using System.Web.Routing;
using Orchard.ContentManagement;

namespace Tobe.Collection.Services {
    public class CurrentContentAccessor : ICurrentContentAccessor {
        private readonly Lazy<ContentItem> _currentContentItemField;
        private readonly IContentManager _contentManager;
        private readonly RequestContext _requestContext;

        public CurrentContentAccessor(IContentManager contentManager, RequestContext requestContext) {
            _contentManager = contentManager;
            _requestContext = requestContext;
            _currentContentItemField = new Lazy<ContentItem>(GetCurrentContentItem);
        }

        public ContentItem CurrentContentItem {
            get { return _currentContentItemField.Value; }
        }

        private ContentItem GetCurrentContentItem() {
            var contentId = GetCurrentContentItemId();
            return contentId == null ? null : _contentManager.Get(contentId.Value);
        }

        private int? GetCurrentContentItemId() {
            return GetContentIdFromAnyRouteValue("id", "blogId");
        }

        private int? GetContentIdFromAnyRouteValue(params string[] keys) {
            foreach (var key in keys) {
                object value;
                if (_requestContext.RouteData.Values.TryGetValue(key, out value)) {
                    int contentId;
                    if (int.TryParse(value as string, out contentId))
                        return contentId;
                }
            }
            return null;
        }
    }
}