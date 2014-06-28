using System;
using System.Collections.Generic;

namespace Zirpl.Spotify.MetadataApi
{
    public class Artist
    {
        public Artist()
        {
            Albums = new List<Album>();
        }

        public String Href { get; set; }
        public String Name { get; set; }
        public String Popularity { get; set; }
        public List<Album> Albums { get; set; }
    }
}