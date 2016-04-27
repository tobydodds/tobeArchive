using System;
using System.Web.Mvc.Routing.Constraints;
using Microsoft.SqlServer.Server;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;

namespace Tobe.Collection.Models {
    public class AlbumPartRecord : ContentPartRecord {
        public virtual AmgRating AmgRating { get; set; }
        public virtual Format Format { get; set; }
        public virtual Length Length { get; set; }
        public virtual string ArtistDisplayName { get; set; }
        public virtual string CatalogNumber { get; set; }
        public virtual string LocalNumber { get; set; }
        public virtual string Description { get; set; }
        public virtual string Notes { get; set; }
        public virtual string OriginalReleaseDate { get; set; }
        public virtual string PurchasedAt { get; set; }
        public virtual DateTime? PurchaseDate { get; set; }
        public virtual string RecordedAt { get; set; }
        public virtual string VersionReleaseDate { get; set; }
        public virtual bool ColoredVinyl { get; set; }
        public virtual bool CoverArtPick { get; set; }
        public virtual bool OriginalPressing { get; set; }
        public virtual bool TobyPick { get; set; }
        public virtual decimal? PricePaid { get; set; }
    }
}