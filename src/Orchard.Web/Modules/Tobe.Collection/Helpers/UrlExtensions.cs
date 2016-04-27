using System.Web.Mvc;
using Tobe.Collection.Models;

namespace Tobe.Collection.Helpers {
    public static class UrlExtensions {
        public static string Agent(this UrlHelper urlHelper, Agent agent) {
            return urlHelper.Action("Details", "Agent", new {id = agent.Id, area = "Tobe.Collection"});
        }
    }
}