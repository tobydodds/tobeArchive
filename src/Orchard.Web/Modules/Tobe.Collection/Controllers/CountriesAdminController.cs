using System.Linq;
using System.Web.Mvc;
using Tobe.Collection.Models;
using Tobe.Collection.Services;
using Tobe.Collection.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Themes;
using System;

namespace Tobe.Collection.Controllers {
    [Admin]
    public class CountriesAdminController : Controller {
        private readonly IOrchardServices _orchardServices;
        private readonly ISiteService _siteService;
        private readonly ICountryManager _countryManager;
        private readonly IRepository<Country> _countryRepository;

        public CountriesAdminController(
            IOrchardServices orchardServices,
            ISiteService siteService,
            IShapeFactory shapeFactory,
            ICountryManager countryManager, 
            IRepository<Country> countryRepository) {

            _orchardServices = orchardServices;
            _siteService = siteService;
            Shape = shapeFactory;
            _countryManager = countryManager;
            _countryRepository = countryRepository;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public dynamic Shape { get; set; }

        public ActionResult Index(PagerParameters pagerParameters) {
            var countryTable = GetCountryTableShape(pagerParameters, "");
            var viewModel = Shape.ViewModel();
            viewModel.CountryTable(countryTable);
            return View(viewModel);
        }

        [Themed(false)]
        public ActionResult Filter(PagerParameters pagerParameters, string filter) {
            var countryTable = GetCountryTableShape(pagerParameters, filter);
            return new ShapeResult(this, countryTable);
        }

        public ActionResult Create() {
            return View(new CreateCountryViewModel());
        }

        [HttpPost]
        public ActionResult Create(CreateCountryViewModel viewModel) {
            if (!ModelState.IsValid) {
                return View(viewModel);
            }
            _countryRepository.Create(new Country
            {
                Name = viewModel.Name
            });
            _orchardServices.Notifier.Add(NotifyType.Information, T("Created the country {0}", viewModel.Name));
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id) {
            var country = _countryRepository.Get(id);
            if (country == null) {
                return new HttpNotFoundResult("Could not find the country with id " + id);
            }
            return View(new EditCountryViewModel {
                Id = id,
                Name = country.Name,
                RelatedContent = _countryManager.GetContentItemsByCountry(id, VersionOptions.Latest).ToArray()
            });
        }

        [HttpPost]
        public ActionResult Edit(EditCountryViewModel viewModel) {
            var country = _countryRepository.Get(viewModel.Id);

            if (!ModelState.IsValid) {
                viewModel.RelatedContent = _countryManager.GetContentItemsByCountry(viewModel.Id, VersionOptions.Latest).ToArray();
                return View("Edit", viewModel);
            }

            country.Name = viewModel.Name;
            _countryRepository.Update(country);
            _orchardServices.Notifier.Add(NotifyType.Information, T("Saved {0}", viewModel.Name));
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            var country = _countryRepository.Get(id);
            if (country == null) {
                return new HttpNotFoundResult("Could not find the country with id " + id);
            }
            _countryRepository.Delete(country);
            _orchardServices.Notifier.Add(NotifyType.Information, T("The country {0} has been deleted", country.Name));
            return RedirectToAction("Index");
        }

        private dynamic GetCountryTableShape(PagerParameters pagerParameters, string filter) {
            var countryTable = Shape.CountryTable();
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            var countryQueryable = _countryRepository.Table;

            if (!String.IsNullOrWhiteSpace(filter)) {
                countryQueryable = countryQueryable.Where(c => c.Name.Contains(filter));
            }
            var countryCount = countryQueryable.Count();
            var query = countryQueryable.OrderBy(c => c.Name).AsQueryable();

            if (pager.PageSize > 0) {
                query = query.Skip((pager.Page - 1) * pager.PageSize).Take(pager.PageSize);
            }

            var countries = query.ToArray().Select(x => new TableRowData<Country> { Data = x, IsDeletable = IsDeletable(x) }).ToList();
            var pagerShape = Shape.Pager(pager).TotalItemCount(countryCount);

            countryTable.Countries(countries);
            countryTable.Pager(pagerShape);

            return countryTable;
        }

        private bool IsDeletable(Country country) {
            return !_countryManager.GetContentItemsByCountry(country.Id, VersionOptions.AllVersions).Any();
        }
    }
}