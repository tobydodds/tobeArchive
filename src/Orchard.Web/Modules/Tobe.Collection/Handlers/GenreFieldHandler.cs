using System;
using System.Linq;
using Tobe.Collection.Fields;
using Tobe.Collection.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.UI.Resources;

namespace Tobe.Collection.Handlers {
    public class GenreFieldHandler : ContentHandler {
        private readonly IGenreManager _genreManager;
        private readonly IResourceManager _resourceManager;

        public GenreFieldHandler(IGenreManager genreManager, IResourceManager resourceManager) {
            _genreManager = genreManager;
            _resourceManager = resourceManager;
        }

        protected override void BuildDisplayShape(BuildDisplayContext context) {
            if (context.DisplayType != "Detail")
                return;

            var genresFields = context.ContentItem.Parts.SelectMany(part => part.Fields.Where(field => field.FieldDefinition.Name == "GenresField")).Cast<GenresField>();
            var genres = genresFields.SelectMany(x => _genreManager.GetGenresByContent(context.ContentItem.Id, x.Name)).Select(x => x.Name).Distinct().ToArray();

            if (!genres.Any())
                return;

            var existingMeta = _resourceManager.GetRegisteredMetas().FirstOrDefault(x => x.Name == "Genre");

            if (existingMeta != null) {
                var genreNames = existingMeta.Content.Split('|').ToList();
                genreNames.AddRange(genres);
                genres = genreNames.Distinct().ToArray();
            }

            _resourceManager.AppendMeta(new MetaEntry {
                Name = "Genre",
                Content = String.Join("|", genres)
            }, "|");
        }

        protected override void Initialized(InitializingContentContext context) {
            var genresFields = context.ContentItem.Parts.SelectMany(x => x.Fields).Where(x => x.FieldDefinition.Name == typeof (GenresField).Name).Select(x => (GenresField)x);

            foreach (var field in genresFields) {
                var closureField = field;
                field._genresField.Loader(() => _genreManager.GetGenresByContent(context.ContentItem.Id, closureField.Name).ToArray());
            }
        }
    }
}