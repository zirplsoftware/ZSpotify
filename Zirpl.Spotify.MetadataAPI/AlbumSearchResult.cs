using System.Collections.Generic;

namespace Zirpl.Spotify.MetadataAPI
{
    public class AlbumSearchResult
    {
        public AlbumSearchResult()
        {
            Albums = new List<Album>();
        }

        public SearchResultInfo Info { get; set; }
        public List<Album> Albums { get; set; }
    }
}