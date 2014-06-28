using System.Collections.Generic;

namespace Zirpl.Spotify.MetadataApi
{
    public class ArtistSearchResult
    {
        public ArtistSearchResult()
        {
            Artists = new List<Artist>();
        }

        public SearchResultInfo Info { get; set; }
        public List<Artist> Artists { get; set; }
    }
}