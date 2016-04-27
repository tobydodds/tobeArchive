using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Downplay.Orchard.LayoutSelector.Services
{
    public class LayoutSelectorService : ILayoutSelectorService
    {

        private readonly IEnumerable<ILayoutAlternatesProvider> _providers;

        public LayoutSelectorService(IEnumerable<ILayoutAlternatesProvider> providers)
        {
            _providers = providers;
        }
        
        public IEnumerable<string> GetLayouts()
        {
            foreach (var p in _providers)
            {
                foreach (var l in p.GetLayouts())
                {
                    yield return l;
                }
            }
        }
    }
}