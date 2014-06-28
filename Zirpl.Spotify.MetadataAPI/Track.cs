using System;
using System.Collections.Generic;

namespace Zirpl.Spotify.MetadataApi
{
    public class Track
    {
        public Track()
        {
            ExternalIds = new List<ExternalId>();
            Artists = new List<Artist>();
        }

        public Album Album { get; set; }
        public String Href { get; set; }
        public String Name { get; set; }
        public String Popularity { get; set; }
        public List<ExternalId> ExternalIds { get; set; }
        public decimal? Length { get; set; }
        public String TrackNumber { get; set; }
        public List<Artist> Artists { get; set; }

        // these are only populated when Album Lookup is done with tracks
        public bool? Available { get; set; }
        public String DiscNumber { get; set; }
        public Availability Availability { get; set; }
    }
}