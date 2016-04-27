using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Tobe.Collection.Models;
using Orchard.Localization;

namespace Tobe.Collection.Helpers {
    public static class HtmlExtensions {
        public static IHtmlString AgentLink(this HtmlHelper htmlHelper, Agent agent, object htmlAttributes = null) {
            return AgentLink(htmlHelper, new LocalizedString(agent.Name), agent, htmlAttributes);
        }

        public static IHtmlString AgentLink(this HtmlHelper htmlHelper, LocalizedString displayText, Agent agent, object htmlAttributes = null) {
            return htmlHelper.ActionLink(String.Format(displayText.ToString(), agent.Name), "Details", "Agent", new { id = agent.Id, area = "Tobe.Collection" }, htmlAttributes);
        }
         public static IHtmlString AgentName(this HtmlHelper htmlHelper, Agent agent, object htmlAttributes = null) {
            return AgentName(htmlHelper, new LocalizedString(agent.Name), agent, htmlAttributes);
        }

        public static IHtmlString AgentName(this HtmlHelper htmlHelper, LocalizedString displayText, Agent agent, object htmlAttributes = null) {
            return htmlHelper.Raw(String.Format(displayText.ToString(), agent.Name));
        }
    }
}