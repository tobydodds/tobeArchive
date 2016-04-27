using System.Linq;
using System.Web.Mvc;
using Tobe.Collection.Models;
using Tobe.Collection.ViewModels;
using Orchard;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Tobe.Collection.Services;
using System;
using Orchard.Themes;

namespace Tobe.Collection.Controllers {
    [Admin]
    public class RolesAdminController : Controller {

        private readonly IOrchardServices _orchardServices;
        private readonly ISiteService _siteService;
        private readonly IRepository<Role> _roleRepository;
        private readonly ICreditsManager _creditsManager;

        public RolesAdminController(
            IOrchardServices orchardServices,
            ISiteService siteService,
            IRepository<Role> roleRepository,
            ICreditsManager creditsManager,
            IShapeFactory shapeFactory) {

            _orchardServices = orchardServices;
            _siteService = siteService;
            _roleRepository = roleRepository;
            _creditsManager = creditsManager;
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public dynamic Shape { get; set; }

        public ActionResult Index(PagerParameters pagerParameters) {
            var roleTable = GetRoleTableShape(pagerParameters, "");
            var viewModel = Shape.ViewModel();
            viewModel.RoleTable(roleTable);
            return View(viewModel);
        }

        [Themed(false)]
        public ActionResult Filter(PagerParameters pagerParameters, string filter) {
            var roleTable = GetRoleTableShape(pagerParameters, filter);
            return new ShapeResult(this, roleTable);
        }

        private dynamic GetRoleTableShape(PagerParameters pagerParameters, string filter) {
            var roleTable = Shape.RoleTable();
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            var roleQueryable = _roleRepository.Table;
            if (!String.IsNullOrWhiteSpace(filter)) {
                roleQueryable = roleQueryable.Where(a => a.Name.Contains(filter));
            }
            var roleCount = roleQueryable.Count();
            var query = roleQueryable.OrderBy(a => a.Name).AsQueryable();

            if (pager.PageSize > 0) {
                query = query.Skip((pager.Page - 1)*pager.PageSize).Take(pager.PageSize);
            }

            var roles = query.ToArray().Select(x => new TableRowData<Role> { Data = x, IsDeletable = IsDeletable(x) }).ToList();
            var pagerShape = Shape.Pager(pager).TotalItemCount(roleCount);

            roleTable.Roles(roles);
            roleTable.Pager(pagerShape);

            return roleTable;
        }

        public ActionResult Create() {
            return View(new CreateRoleViewModel());
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST(CreateRoleViewModel viewModel, string returnUrl) {

            if (!ModelState.IsValid) {
                return View(viewModel);
            }
            _roleRepository.Create(new Role
            {
                Name = viewModel.Name
            });
            _orchardServices.Notifier.Add(NotifyType.Information, T("Created the role {0}", viewModel.Name));
            return Url.IsLocalUrl(returnUrl) ? (ActionResult)Redirect(returnUrl) : RedirectToAction("Index");
        }

        public ActionResult Edit(int id) {
            var role = _roleRepository.Get(id);
            if (role == null) {
                return new HttpNotFoundResult("Could not find the role with id " + id);
            }
            return View(new EditRoleViewModel
            {
                Id = id, Name = role.Name
            });
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(EditRoleViewModel viewModel) {
            var role = _roleRepository.Get(viewModel.Id);

            role.Name = viewModel.Name;
            _roleRepository.Update(role);
            _orchardServices.Notifier.Add(NotifyType.Information, T("Saved {0}", viewModel.Name));
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            var role = _roleRepository.Get(id);
            if (role == null) {
                return new HttpNotFoundResult("Could not find the role with id " + id);
            }
            if (!IsDeletable(role)) {
                throw new InvalidOperationException("This Role is being referenced by at least one Credit.");
            }
            _creditsManager.DeleteRole(role);
            _orchardServices.Notifier.Add(NotifyType.Information, T("The role {0} has been deleted", role.Name));
            return RedirectToAction("Index");
        }

        private bool IsDeletable(Role role) {
            return !_creditsManager.GetCreditsByRole(role.Id).Any();
        }
    }
}