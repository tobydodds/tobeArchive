﻿using System;
using System.Web.Mvc;
﻿using Orchard;
﻿using Orchard.Mvc.Filters;
using Orchard.Themes.Services;
using Orchard.UI.Admin;
using Orchard.UI.Resources;
using Bootstrap.Services;
using System.Linq;

namespace Bootstrap.Filters {
    public class LayoutFilter : FilterProvider, IResultFilter {
        private readonly IThemeSettingsService _settingsService;
        private readonly IResourceManager _resourceManager;
        private readonly ISiteThemeService _siteThemeService;
        private readonly IWorkContextAccessor _wca;

        public LayoutFilter(IThemeSettingsService settingsService, IResourceManager resourceManager, ISiteThemeService siteThemeService, IWorkContextAccessor wca) {
            _settingsService = settingsService;
            _resourceManager = resourceManager;
            _siteThemeService = siteThemeService;
            _wca = wca;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext) {

            // ignore filter on admin pages
            if (AdminFilter.IsApplied(filterContext.RequestContext))
                return;

            // should only run on a full view rendering result
            if (!(filterContext.Result is ViewResult))
                return;

            var settings = _settingsService.GetSettings();

            if (String.IsNullOrEmpty(settings.Swatch))
                return;

            var themeName = _siteThemeService.GetSiteTheme();
            if (themeName.Name == Constants.ThemeName) {
                var viewResult = filterContext.Result as ViewResult;
                if (viewResult == null)
                    return;

                if (settings.UseFixedNav) {
                    /* TODO: Replace note use Items collection */
                    System.Web.HttpContext.Current.Items[Constants.UseFixedNav] = settings.UseFixedNav.ToString();
                }
                if (settings.UseNavSearch) {
                    /* TODO: Replace note use Items collection */
                    System.Web.HttpContext.Current.Items[Constants.UseNavSearch] = settings.UseNavSearch.ToString();
                }
                if (settings.UseFluidLayout) {
                    /* TODO: Replace note use Items collection */
                    System.Web.HttpContext.Current.Items[Constants.UseFluidLayout] = settings.UseFluidLayout.ToString();
                }
                if (settings.UseInverseNav) {
                    /* TODO: Replace note use Items collection */
                    System.Web.HttpContext.Current.Items[Constants.UseInverseNav] = settings.UseInverseNav.ToString();
                }
                if (settings.UseStickyFooter) {
                    /* TODO: Replace note use Items collection */
                    System.Web.HttpContext.Current.Items[Constants.UseStickyFooter] = settings.UseStickyFooter.ToString();
                }
                var workContext = _wca.GetContext();
                //check if this is the login screen, look for layout-logon.cshtml layout
                if ((new string[] { "~/Users/Account/LogOn", "~/Users/Account/AccessDenied" }).Contains(((workContext.HttpContext).Request).AppRelativeCurrentExecutionFilePath))
                {
                    workContext.Layout.Metadata.Alternates.Add("Layout__LogOn");
                }
            
            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {
        }
    }
}
