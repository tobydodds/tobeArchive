using System;
using Tobe.Collection.Models;
using Orchard;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;

namespace Tobe.Collection.ImportExport {
    [OrchardFeature("Tobe.Collection.ImportExport")]
    public partial class TobeImportHandler : Component, IRecipeHandler {
        private readonly IRepository<Agent> _agentRepository;
        private readonly IRepository<Country> _countryRepository;

        public TobeImportHandler(IRepository<Agent> agentRepository, 
            IRepository<Country> countryRepository) {
            _agentRepository = agentRepository;
            _countryRepository = countryRepository;
            }

        public void ExecuteRecipeStep(RecipeContext recipeContext) {
            if (!String.Equals(recipeContext.RecipeStep.Name, "TobeGlossary", StringComparison.OrdinalIgnoreCase)) {
                return;
            }

            ImportAgents(recipeContext.RecipeStep.Step);
            ImportCountries(recipeContext.RecipeStep.Step);

            recipeContext.Executed = true;
        }
    }
}
