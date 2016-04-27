using System.Linq;
using System.Xml.Linq;
using Tobe.Collection.Fields;
using Tobe.Collection.Models;
using Tobe.Collection.Services;
using Tobe.Collection.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace Tobe.Collection.Drivers {
    public class GenresFieldDriver : ContentFieldDriver<GenresField> {
        private readonly IGenreManager _genreManager;

        public GenresFieldDriver(IGenreManager genreManager) {
            _genreManager = genreManager;
        }

        protected override DriverResult Display(ContentPart part, GenresField field, string displayType, dynamic shapeHelper) {
            return ContentShape("Fields_Genres", GetDifferentiator(field), () => shapeHelper.Fields_Genres(Genres: _genreManager.GetGenresByContent(part.Id, field.Name).ToArray()));
        }

        protected override DriverResult Editor(ContentPart part, GenresField field, dynamic shapeHelper) {
            return Editor(part, field, null, shapeHelper);
        }

        protected override DriverResult Editor(ContentPart part, GenresField field, IUpdateModel updater, dynamic shapeHelper) {
            return ContentShape("Fields_Genres_Edit", GetDifferentiator(field), () => {
                var prefix = GetPrefix(field, part);
                var model = new GenresViewModel {
                    AllGenres = _genreManager.Query().ToArray(),
                    ContentField = field
                };

                if (updater != null) {
                    if (updater.TryUpdateModel(model, prefix, null, null)) {
                        UpdateGenres(part, field, model);
                    }
                }

                var genres = _genreManager.GetGenresByContent(part.Id, field.Name);
                model.Genres = genres.Select(x => new GenreViewModel {
                    Id = x.Id,
                    Name = x.Name
                }).ToArray();
                return shapeHelper.EditorTemplate(TemplateName: "Fields/Genres", Model: model, Prefix: prefix);
            });
        }

        protected override void Exporting(ContentPart part, GenresField field, ExportContentContext context) {
            var fieldElement = context.Element(field.FieldDefinition.Name + "." + field.Name);
            var genres = _genreManager.GetGenresByContent(part.Id, field.Name).ToArray();

            if (genres.Any()) {
                fieldElement.Add(genres.Select(x => new XElement("Genre", new XAttribute("Name", x.Name))));
            }
        }

        protected override void Importing(ContentPart part, GenresField field, ImportContentContext context) {
            var fieldElement = context.Data.Element(field.FieldDefinition.Name + "." + field.Name);

            if (fieldElement == null)
                return;

            var genresDictionary = _genreManager.Query().ToDictionary(x => x.Name);

            foreach (var genre in fieldElement.Elements("Genre")
                .Select(genreElement => genreElement.Attr<string>("Name"))
                .Select(genreName => genresDictionary.ContainsKey(genreName) ? genresDictionary[genreName] : default(Genre))
                .Where(genre => genre != null)) {
                    _genreManager.AddGenreToContent(part.ContentItem, field.Name, genre);
            }
        }

        private void UpdateGenres(ContentPart part, GenresField field, GenresViewModel model) {
            var postedGenreIds = model.Genres != null ? model.Genres.Select(x => x.Id).ToArray() : Enumerable.Empty<int>();
            var postedGenres = _genreManager.Query().Where(x => postedGenreIds.Contains(x.Id)).ToArray();
            var genres = _genreManager.GetGenresByContent(part.Id, field.Name);

            _genreManager.RemoveGenresFromContent(part.Id, field.Name, postedGenreIds);

            foreach (var postedGenre in
                    from postedGenre in postedGenres
                    let genre = genres.FirstOrDefault(x => x.Id == postedGenre.Id)
                    where genre == null
                    select postedGenre) {
                _genreManager.AddGenreToContent(part.ContentItem, field.Name, postedGenre);
            }
        }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(ContentField field) {
            return field.Name;
        }
    }
}