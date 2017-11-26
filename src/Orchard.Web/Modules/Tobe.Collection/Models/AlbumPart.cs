using System;
using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Core.Common.Utilities;
using Orchard.Core.Title.Models;

namespace Tobe.Collection.Models {
    public class AlbumPart : ContentPart<AlbumPartRecord>, ICreditDisplay {
        internal LazyField<IEnumerable<Agent>> _allAgentsField = new LazyField<IEnumerable<Agent>>();
        internal LazyField<IEnumerable<Agent>> _allArtistsField = new LazyField<IEnumerable<Agent>>();
        internal LazyField<IEnumerable<Genre>> _genresField = new LazyField<IEnumerable<Genre>>();

        public string Name {
            get { return this.As<TitlePart>().Title; }
            set { this.As<TitlePart>().Title = value; }
        }

         public AmgRating AmgRating {
            get { return Record.AmgRating; }
            set { Record.AmgRating = value; }
        }

         public Format Format {
            get { return Record.Format; }
            set { Record.Format = value; }
        }

        public Length Length {
            get { return Record.Length; }
            set { Record.Length = value; }
        }

        public string ArtistDisplayName {
            get { return Record.ArtistDisplayName; }
            set { Record.ArtistDisplayName = value; }
        }

        public string ArtistNameSort {
            get { return Record.ArtistNameSort; }
            set { Record.ArtistNameSort = value; }
        }

        public string CatalogNumber {
            get { return Record.CatalogNumber; }
            set { Record.CatalogNumber = value; }
        }

        public string LocalNumber {
            get { return Record.LocalNumber; }
            set { Record.LocalNumber = value; }
        }

        public string Description {
            get { return Record.Description; }
            set { Record.Description = value; }
        }

        public string Notes {
            get { return Record.Notes; }
            set { Record.Notes = value; }
        }

        public string OriginalReleaseDate {
            get { return Record.OriginalReleaseDate; }
            set { Record.OriginalReleaseDate = value; }
        }

        public string PurchasedAt {
            get { return Record.PurchasedAt; }
            set { Record.PurchasedAt = value; }
        }

        public DateTime? PurchaseDate {
            get { return Record.PurchaseDate; }
            set { Record.PurchaseDate = value; }
        }

        public string RecordedAt {
            get { return Record.RecordedAt; }
            set { Record.RecordedAt = value; }
        }

        public string VersionReleaseDate {
            get { return Record.VersionReleaseDate; }
            set { Record.VersionReleaseDate = value; }
        }

        public bool ColoredVinyl {
            get { return Record.ColoredVinyl; }
            set { Record.ColoredVinyl = value; }
        }

        public bool CoverArtPick {
            get { return Record.CoverArtPick; }
            set { Record.CoverArtPick = value; }
        }

        public bool OriginalPressing {
            get { return Record.OriginalPressing; }
            set { Record.OriginalPressing = value; }
        }

        public bool TobyPick {
            get { return Record.TobyPick; }
            set { Record.TobyPick = value; }
        }

        public decimal? PricePaid {
            get { return Record.PricePaid; }
            set { Record.PricePaid = value; }
        }

        public IEnumerable<Agent> AllAgents {
            get { return _allAgentsField.Value; }
        }

        public IEnumerable<Genre> Genres {
            get { return _genresField.Value; }
        }
    }
}