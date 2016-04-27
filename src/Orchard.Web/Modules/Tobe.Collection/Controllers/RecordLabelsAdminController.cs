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
    public class RecordLabelsAdminController : Controller {
        private readonly IOrchardServices _orchardServices;
        private readonly ISiteService _siteService;
        private readonly IRecordLabelManager _recordLabelManager;
        private readonly IRepository<RecordLabel> _recordLabelRepository;

        public RecordLabelsAdminController(
            IOrchardServices orchardServices,
            ISiteService siteService,
            IShapeFactory shapeFactory,
            IRecordLabelManager recordLabelManager, 
            IRepository<RecordLabel> recordLabelRepository) {

            _orchardServices = orchardServices;
            _siteService = siteService;
            Shape = shapeFactory;
            _recordLabelManager = recordLabelManager;
            _recordLabelRepository = recordLabelRepository;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public dynamic Shape { get; set; }

        public ActionResult Index(PagerParameters pagerParameters) {
            var recordLabelTable = GetRecordLabelTableShape(pagerParameters, "");
            var viewModel = Shape.ViewModel();
            viewModel.RecordLabelTable(recordLabelTable);
            return View(viewModel);
        }

        [Themed(false)]
        public ActionResult Filter(PagerParameters pagerParameters, string filter) {
            var recordLabelTable = GetRecordLabelTableShape(pagerParameters, filter);
            return new ShapeResult(this, recordLabelTable);
        }

        public ActionResult Create() {
            return View(new CreateRecordLabelViewModel());
        }

        [HttpPost]
        public ActionResult Create(CreateRecordLabelViewModel viewModel) {
            if (!ModelState.IsValid) {
                return View(viewModel);
            }
            _recordLabelRepository.Create(new RecordLabel
            {
                Name = viewModel.Name
            });
            _orchardServices.Notifier.Add(NotifyType.Information, T("Created the recordLabel {0}", viewModel.Name));
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id) {
            var recordLabel = _recordLabelRepository.Get(id);
            if (recordLabel == null) {
                return new HttpNotFoundResult("Could not find the recordLabel with id " + id);
            }
            return View(new EditRecordLabelViewModel {
                Id = id,
                Name = recordLabel.Name,
                RelatedContent = _recordLabelManager.GetContentItemsByRecordLabel(id, VersionOptions.Latest).ToArray()
            });
        }

        [HttpPost]
        public ActionResult Edit(EditRecordLabelViewModel viewModel) {
            var recordLabel = _recordLabelRepository.Get(viewModel.Id);

            if (!ModelState.IsValid) {
                viewModel.RelatedContent = _recordLabelManager.GetContentItemsByRecordLabel(viewModel.Id, VersionOptions.Latest).ToArray();
                return View("Edit", viewModel);
            }

            recordLabel.Name = viewModel.Name;
            _recordLabelRepository.Update(recordLabel);
            _orchardServices.Notifier.Add(NotifyType.Information, T("Saved {0}", viewModel.Name));
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            var recordLabel = _recordLabelRepository.Get(id);
            if (recordLabel == null) {
                return new HttpNotFoundResult("Could not find the recordLabel with id " + id);
            }
            _recordLabelRepository.Delete(recordLabel);
            _orchardServices.Notifier.Add(NotifyType.Information, T("The recordLabel {0} has been deleted", recordLabel.Name));
            return RedirectToAction("Index");
        }

        private dynamic GetRecordLabelTableShape(PagerParameters pagerParameters, string filter) {
            var recordLabelTable = Shape.RecordLabelTable();
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            var recordLabelQueryable = _recordLabelRepository.Table;

            if (!String.IsNullOrWhiteSpace(filter)) {
                recordLabelQueryable = recordLabelQueryable.Where(c => c.Name.Contains(filter));
            }
            var recordLabelCount = recordLabelQueryable.Count();
            var query = recordLabelQueryable.OrderBy(c => c.Name).AsQueryable();

            if (pager.PageSize > 0) {
                query = query.Skip((pager.Page - 1) * pager.PageSize).Take(pager.PageSize);
            }

            var recordLabels = query.ToArray().Select(x => new TableRowData<RecordLabel> { Data = x, IsDeletable = IsDeletable(x) }).ToList();
            var pagerShape = Shape.Pager(pager).TotalItemCount(recordLabelCount);

            recordLabelTable.RecordLabels(recordLabels);
            recordLabelTable.Pager(pagerShape);

            return recordLabelTable;
        }

        private bool IsDeletable(RecordLabel recordLabel) {
            return !_recordLabelManager.GetContentItemsByRecordLabel(recordLabel.Id, VersionOptions.AllVersions).Any();
        }
    }
}