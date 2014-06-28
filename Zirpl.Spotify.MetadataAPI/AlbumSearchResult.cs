using System.Collections.Generic;

namespace Zirpl.Spotify.MetadataApi
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