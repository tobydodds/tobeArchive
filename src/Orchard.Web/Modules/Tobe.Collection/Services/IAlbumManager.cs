using System;
using System.Collections.Generic;
using Tobe.Collection.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Tobe.Collection.Services {
    public interface IAlbumManager : IDependency {
        AlbumPart GetAlbum(int id, VersionOptions versionOptions);
        AmgRating GetAmgRating(int id);
        AmgRating GetAmgRatingByName(string name);
        IEnumerable<AmgRating> GetAmgRatings();
        IEnumerable<AlbumPart> GetAlbumsByAmgRating(int amgRatingId, VersionOptions versionOptions = null);
        Format GetFormat(int id);
        Format GetFormatByName(string name);
        IEnumerable<Format> GetFormats();
        IEnumerable<AlbumPart> GetAlbumsByFormat(int formatId, VersionOptions versionOptions = null);
        Length GetLength(int id);
        Length GetLengthByName(string name);
        IEnumerable<Length> GetLengths();
        IEnumerable<AlbumPart> GetAlbumsByLength(int lengthId, VersionOptions versionOptions = null);
            
        /// <summary>
        /// Returns all agent artists associated with the specified album on every level.
        /// </summary>
        IEnumerable<Agent> GetAllAgentArtistsFor(AlbumPart album);

        /// <summary>
        /// Returns all agent artists associated with the specified album except for album artists.
        /// </summary>

        IEnumerable<AlbumAgentArtistRelation> GetAllAlbumArtistsByAgent(int agentId, VersionOptions versionOptions = null);

        /// <summary>
        /// Returns all agent credits associated with the specified album on every level.
        /// </summary>
        IEnumerable<Agent> GetAllAgentCreditsFor(AlbumPart album);

        /// <summary>
        /// Returns all agent credits associated with the specified album except for album credits.
        /// </summary>

        IEnumerable<AlbumAgentCreditRelation> GetAllAlbumCreditsByAgent(int agentId, VersionOptions versionOptions = null);
    }
}