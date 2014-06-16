using System.Collections.Generic;

namespace Zirpl.Spotify.MetadataAPI
{
    public class TrackSearchResult
    {
        public TrackSearchResult()
        {
            Tracks = new List<Track>();
        }

        public SearchResultInfo Info { get; set; }
        public List<Track> Tracks { get; set; }
    }
}