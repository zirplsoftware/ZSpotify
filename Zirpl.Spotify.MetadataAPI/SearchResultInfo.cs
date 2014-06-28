using System;
#if !REF_RESTSHARP
using Newtonsoft.Json;
#endif

namespace Zirpl.Spotify.MetadataApi
{
    public class SearchResultInfo
    {
#if !REF_RESTSHARP
        [JsonProperty(PropertyName = "num_results")]
#endif
        public int? NumResults { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
        public String Query { get; set; }
        public String Type { get; set; }
        public int? Page { get; set; }
    }
}