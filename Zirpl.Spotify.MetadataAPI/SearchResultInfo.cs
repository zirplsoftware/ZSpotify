using System;

namespace Zirpl.Spotify.MetadataApi
{
    public class SearchResultInfo
    {
        public int? NumResults { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
        public String Query { get; set; }
        public String Type { get; set; }
        public int? Page { get; set; }
    }
}