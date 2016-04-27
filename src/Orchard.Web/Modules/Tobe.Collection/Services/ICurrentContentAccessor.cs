using Orchard;
using Orchard.ContentManagement;

namespace Tobe.Collection.Services {
    public interface ICurrentContentAccessor : IDependency {
        ContentItem CurrentContentItem { get; }
    }
}