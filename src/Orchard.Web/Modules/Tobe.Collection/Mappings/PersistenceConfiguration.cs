using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using Tobe.Collection.Models;
using NHibernate.Cfg;
using Orchard.Data;
using Orchard.Utility;

namespace Tobe.Collection.Mappings {
    public class PersistenceConfiguration : ISessionConfigurationEvents {
        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel) {
            defaultModel.Override<AlbumPartRecord>(mapping => {
                mapping.References(x => x.AmgRating, "AmgRatingId");
                mapping.References(x => x.Format, "FormatId");
                mapping.References(x => x.Length, "LengthId");
            });
            defaultModel.Override<ContentCountries>(mapping => {
                mapping.References(x => x.ContentItem, "ContentItemId");
                mapping.References(x => x.Country, "CountryId");
            });
            defaultModel.Override<ContentGenres>(mapping => {
                mapping.References(x => x.ContentItem, "ContentItemId");
                mapping.References(x => x.Genre, "GenreId");
            });
            defaultModel.Override<ContentRecordLabels>(mapping => {
                mapping.References(x => x.ContentItem, "ContentItemId");
                mapping.References(x => x.RecordLabel, "RecordLabelId");
            });
        }
        public void Prepared(FluentConfiguration cfg) {}
        public void Building(Configuration cfg) {}
        public void Finished(Configuration cfg) {}

        public void ComputingHash(Hash hash) {
            hash.AddString("7C44D52B-35F1-4104-BBFD-9E16C444119F");
        }
    }
}