using System.Collections.Generic;
using Orchard.Environment.Extensions;
using Orchard.ImportExport.Services;

namespace Tobe.Collection.ImportExport {
    [OrchardFeature("Tobe.Collection.ImportExport")]
    public class WorkflowsCustomExportStep : ICustomExportStep {
        public void Register(IList<string> steps) {
            steps.Add("TobeGlossary");
        }
    }
}