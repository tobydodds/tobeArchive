using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Downplay.Orchard.LayoutSelector.Services
{
    public class LayoutAlternatesProvider : ILayoutAlternatesProvider
    {
        public IEnumerable<string> GetLayouts()
        {
            yield return "Default";
            yield return "Homepage";
        }
    } 
}