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
    public class GenresAdminController : Controller {
        private readonly IOrchardServices _orchardServices;
        private readonly ISiteService _siteService;
        private readonly IGenreManager _genreManager;
        private readonly IRepository<Genre> _genreRepository;

        public GenresAdminController(
            IOrchardServices orchardServices,
            ISiteService siteService,
            IShapeFactory shapeFactory,
            IGenreManager genreManager, 
            IRepository<Genre> genreRepository) {

            _orchardServices = orchardServices;
            _siteService = siteService;
            Shape = shapeFactory;
            _genreManager = genreManager;
            _genreRepository = genreRepository;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public dynamic Shape { get; set; }

        public ActionResult Index(PagerParameters pagerParameters) {
            var genreTable = GetGenreTableShape(pagerParameters, "");
            var viewModel = Shape.ViewModel();
            viewModel.GenreTable(genreTable);
            return View(viewModel);
        }

        [Themed(false)]
        public ActionResult Filter(PagerParameters pagerParameters, string filter) {
            var genreTable = GetGenreTableShape(pagerParameters, filter);
            return new ShapeResult(this, genreTable);
        }

        public ActionResult Create() {
            return View(new CreateGenreViewModel());
        }

        [HttpPost]
        public ActionResult Create(CreateGenreViewModel viewModel) {
            if (!ModelState.IsValid) {
                return View(viewModel);
            }
            _genreRepository.Create(new Genre
            {
                Name = viewModel.Name
            });
            _orchardServices.Notifier.Add(NotifyType.Information, T("Created the genre {0}", viewModel.Name));
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id) {
            var genre = _genreRepository.Get(id);
            if (genre == null) {
                return new HttpNotFoundResult("Could not find the genre with id " + id);
            }
            return View(new EditGenreViewModel {
                Id = id,
                Name = genre.Name,
                RelatedContent = _genreManager.GetContentItemsByGenre(id, VersionOptions.Latest).ToArray()
            });
        }

        [HttpPost]
        public ActionResult Edit(EditGenreViewModel viewModel) {
            var genre = _genreRepository.Get(viewModel.Id);

            if (!ModelState.IsValid) {
                viewModel.RelatedContent = _genreManager.GetContentItemsByGenre(viewModel.Id, VersionOptions.Latest).ToArray();
                return View("Edit", viewModel);
            }

            genre.Name = viewModel.Name;
            _genreRepository.Update(genre);
            _orchardServices.Notifier.Add(NotifyType.Information, T("Saved {0}", viewModel.Name));
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            var genre = _genreRepository.Get(id);
            if (genre == null) {
                return new HttpNotFoundResult("Could not find the genre with id " + id);
            }
            _genreRepository.Delete(genre);
            _orchardServices.Notifier.Add(NotifyType.Information, T("The genre {0} has been deleted", genre.Name));
            return RedirectToAction("Index");
        }

        private dynamic GetGenreTableShape(PagerParameters pagerParameters, string filter) {
            var genreTable = Shape.GenreTable();
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            var genreQueryable = _genreRepository.Table;

            if (!String.IsNullOrWhiteSpace(filter)) {
                genreQueryable = genreQueryable.Where(c => c.Name.Contains(filter));
            }
            var genreCount = genreQueryable.Count();
            var query = genreQueryable.OrderBy(c => c.Name).AsQueryable();

            if (pager.PageSize > 0) {
                query = query.Skip((pager.Page - 1) * pager.PageSize).Take(pager.PageSize);
            }

            var genres = query.ToArray().Select(x => new TableRowData<Genre> { Data = x, IsDeletable = IsDeletable(x) }).ToList();
            var pagerShape = Shape.Pager(pager).TotalItemCount(genreCount);

            genreTable.Genres(genres);
            genreTable.Pager(pagerShape);

            return genreTable;
        }

        private bool IsDeletable(Genre genre) {
            return !_genreManager.GetContentItemsByGenre(genre.Id, VersionOptions.AllVersions).Any();
        }
    }
}