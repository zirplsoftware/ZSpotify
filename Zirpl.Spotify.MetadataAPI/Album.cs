using System;
using System.Collections.Generic;
#if !REF_RESTSHARP
using Newtonsoft.Json;
#endif

namespace Zirpl.Spotify.MetadataApi
{
    public class Album
    {
        public Album()
        {
            ExternalIds = new List<ExternalId>();
            Artists = new List<Artist>();
        }

        public string Href { get; set; }
        public String Name { get; set; }
        public string Popularity { get; set; }
#if !REF_RESTSHARP
        [JsonProperty(PropertyName = "external-ids")]
#endif
        public List<ExternalId> ExternalIds { get; set; }
        public List<Artist> Artists { get; set; }
        public Availability Availability { get; set; }

        public String Released { get; set; }

        // these are only populated when extra=album is used with an Artist lookup
#if !REF_RESTSHARP
        [JsonProperty(PropertyName = "artist-id")]
#endif
        public String ArtistId { get; set; }
        public String Artist { get; set; }
    }
}