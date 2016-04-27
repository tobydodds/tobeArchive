using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;

namespace Downplay.Orchard.LayoutSelector.Models
{
    public class LayoutSelectorPart : ContentPart<LayoutSelectorPartRecord>
    {

        public string LayoutName
        {
            get { return Record.LayoutName; }
            set { Record.LayoutName = value; }
        }

        /// <summary>
        /// This will be populated from the driver.
        /// TODO: I saw an example of this in another module; is this Best Practice or is there a better way to provide this kind of data to the UI?
        /// </summary>
        public IEnumerable<String> AvailableLayouts { get; set; }

    }
}