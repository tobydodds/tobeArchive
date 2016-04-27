using System;
using System.Linq;
using System.Web.Mvc;
using Tobe.Collection.Models;
using Tobe.Collection.Services;
using Tobe.Collection.ViewModels;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Themes;

namespace Tobe.Collection.Controllers {
    [Themed]
    public class AgentController : Controller {
        private readonly IRepository<Agent> _agentRepository;
        private readonly ICreditsManager _creditsManager;
        private readonly IAlbumManager _albumManager;

        public AgentController(IRepository<Agent> agentRepository, ICreditsManager creditsManager, IAlbumManager albumManager) {
            _agentRepository = agentRepository;
            _creditsManager = creditsManager;
            _albumManager = albumManager;
        }

        [HttpGet]
        public ActionResult Details(int id) {
            return AgentDetails(() => _agentRepository.Get(id));
        }

        [HttpGet]
        public ActionResult DetailsByName(string agentName) {
            return AgentDetails(() => _agentRepository.Fetch(a => a.Name == agentName).SingleOrDefault());
        }

        public JsonResult Search(string term, bool nameOnly = false) {
            var query = _agentRepository.Table;

            if (!String.IsNullOrWhiteSpace(term)) {
                query = query.Where(a => a.Name.Contains(term));
            }

            return nameOnly 
                ? Json(query.Select(x => new { value = x.Name, label = x.Name }).ToArray(), JsonRequestBehavior.AllowGet) 
                : Json(query.Select(x => new {value = x.Id, label = x.Name}).ToArray(), JsonRequestBehavior.AllowGet);
        }

        private ActionResult AgentDetails(Func<Agent> getAgent) {
            var agent = getAgent();
            if (agent == null) return HttpNotFound();
            var creditContainers = _creditsManager.GetCreditsContainersByAgent(agent.Id).ToArray();
            var albumArtists = _albumManager.GetAllAlbumArtistsByAgent(agent.Id, VersionOptions.Latest).ToArray();
            var albumCredits = _albumManager.GetAllAlbumCreditsByAgent(agent.Id, VersionOptions.Latest).ToArray();
            var viewModel = new AgentViewModel {
                Id = agent.Id,
                Name = agent.Name,
                NameSort = agent.NameSort,
                FileUnder = agent.FileUnder,
                CreditContainers = creditContainers,
                RelatedAlbumArtists = albumArtists,
                RelatedAlbumCredits = albumCredits
            };
            return View("Details", viewModel);
        }
    }
}