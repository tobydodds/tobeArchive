using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using Tobe.Collection.Models;

namespace Tobe.Collection.ViewModels {
    public class AlbumViewModel { 
        [UIHint("AmgRatingPicker")]
        public int? AmgRatingId { get; set; }
        [UIHint("FormatPicker")]
        public int? FormatId { get; set; }
        [UIHint("LengthPicker")]
        public int? LengthId { get; set; }
        [MaxLength(300)]
        public string ArtistDisplayName { get; set; }
        public string CatalogNumber { get; set; }
        public string LocalNumber { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public string OriginalReleaseDate { get; set; }
        public string PurchasedAt { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string RecordedAt { get; set; }
        public string VersionReleaseDate { get; set; }
        public bool ColoredVinyl { get; set; }
        public bool CoverArtPick { get; set; }
        public bool OriginalPressing { get; set; }
        public bool TobyPick { get; set; }
        public decimal? PricePaid { get; set; }

        public IList<AmgRating> AvailableAmgRatings { get; set; }
        public IList<Format> AvailableFormats { get; set; }
        public IList<Length> AvailableLengths { get; set; }
    }
}