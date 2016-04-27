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
using System;
using Orchard.Themes;

namespace Tobe.Collection.Controllers {
    [Admin]
    public class AgentsAdminController : Controller {

        private readonly IOrchardServices _orchardServices;
        private readonly ISiteService _siteService;
        private readonly IRepository<Agent> _agentRepository;
        private readonly ICreditsManager _creditsManager;
        private readonly IAlbumManager _albumManager;

        public AgentsAdminController(
            IOrchardServices orchardServices,
            ISiteService siteService,
            IRepository<Agent> agentRepository,
            IShapeFactory shapeFactory,
            ICreditsManager creditsManager, 
            IAlbumManager albumManager) {

            _orchardServices = orchardServices;
            _siteService = siteService;
            _agentRepository = agentRepository;
            Shape = shapeFactory;
            _creditsManager = creditsManager;
            _albumManager = albumManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public dynamic Shape { get; set; }

        public ActionResult Index(PagerParameters pagerParameters) {
            var agentTable = GetAgentTableShape(pagerParameters, "");
            var viewModel = Shape.ViewModel();
            viewModel.AgentTable(agentTable);
            return View(viewModel);
        }

        [Themed(false)]
        public ActionResult Filter(PagerParameters pagerParameters, string filter) {
            var agentTable = GetAgentTableShape(pagerParameters, filter);
            return new ShapeResult(this, agentTable);
        }

        public ActionResult Create() {
            return View(new CreateAgentViewModel());
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Create(CreateAgentViewModel viewModel, string returnUrl) {
            if (!ModelState.IsValid) {
                return View(viewModel);
            }
            _agentRepository.Create(new Agent
            {
                Name = viewModel.Name, 
                NameSort = viewModel.NameSort, 
                FileUnder = viewModel.FileUnder
            });
            _orchardServices.Notifier.Add(NotifyType.Information, T("Created the agent {0}", viewModel.Name));
            return Url.IsLocalUrl(returnUrl) ? (ActionResult) Redirect(returnUrl) : RedirectToAction("Index");
        }

        public ActionResult Edit(int id) {
            var agent = _agentRepository.Get(id);
            if (agent == null) {
                return new HttpNotFoundResult("Could not find the agent with id " + id);
            }
            var albumArtists = _albumManager.GetAllAlbumArtistsByAgent(agent.Id, VersionOptions.Latest).ToArray();
            var albumCredits = _albumManager.GetAllAlbumCreditsByAgent(agent.Id, VersionOptions.Latest).ToArray();
            var relatedContent = _creditsManager.GetCreditsContainersByAgent(id, VersionOptions.Latest).Where(x => !x.Is<AlbumPart>()).ToArray();
            return View(new EditAgentViewModel {
                Id = id, 
                Name = agent.Name, 
                NameSort = agent.NameSort,
                FileUnder = agent.FileUnder,
                RelatedAlbumArtists = albumArtists,
                RelatedAlbumCredits = albumCredits,
                RelatedContent = relatedContent
            });
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult Edit(EditAgentViewModel viewModel) {
            var agent = _agentRepository.Get(viewModel.Id);

            if (!ModelState.IsValid) {
                viewModel.RelatedAlbumArtists = _albumManager.GetAllAlbumArtistsByAgent(agent.Id, VersionOptions.Latest).ToArray();
                return View("Edit", viewModel);
            }

            if (!ModelState.IsValid) {
                viewModel.RelatedAlbumCredits = _albumManager.GetAllAlbumCreditsByAgent(agent.Id, VersionOptions.Latest).ToArray();
                return View("Edit", viewModel);
            }

            agent.Name = viewModel.Name;
            agent.NameSort = viewModel.NameSort;
            agent.FileUnder = viewModel.FileUnder;
            _agentRepository.Update(agent);
            _orchardServices.Notifier.Add(NotifyType.Information, T("Saved {0}", viewModel.Name));
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            var agent = _agentRepository.Get(id);
            if (agent == null) {
                return new HttpNotFoundResult("Could not find the agent with id " + id);
            }
            if (!IsDeletable(agent)) {
                throw new InvalidOperationException("This Agent is being referenced by at least one Credit.");
            }
            _creditsManager.DeleteAgent(agent);
            _orchardServices.Notifier.Add(NotifyType.Information, T("The agent {0} has been deleted", agent.Name));
            return RedirectToAction("Index");
        }

        private dynamic GetAgentTableShape(PagerParameters pagerParameters, string filter) {
            var agentTable = Shape.AgentTable();
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            var agentQueryable = _agentRepository.Table;

            if (!String.IsNullOrWhiteSpace(filter)) {
                agentQueryable = agentQueryable.Where(a => a.Name.Contains(filter));
            }
            var agentCount = agentQueryable.Count();
            var query = agentQueryable.OrderBy(x => x.Name).AsQueryable();

            if (pager.PageSize > 0)
                query = query.Skip((pager.Page - 1) * pager.PageSize).Take(pager.PageSize);

            var agents = query.ToArray().Select(x => new TableRowData<Agent> { Data = x, IsDeletable = IsDeletable(x) }).ToList();
            var pagerShape = Shape.Pager(pager).TotalItemCount(agentCount);

            agentTable.Agents(agents);
            agentTable.Pager(pagerShape);

            return agentTable;
        }

        private bool IsDeletable(Agent agent) {
            return
                !_creditsManager.GetCreditsByAgent(agent.Id).Any();
        }
    }
}