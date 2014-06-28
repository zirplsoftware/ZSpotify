using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Zirpl.Spotify.MetadataApi;

namespace Zirpl.Spotify.MetadataAPI.Tests
{
    [TestFixture]
    public class SpotifyMetadataApiClientTests
    {
        [Test]
        public void TestRateLimitingWorks()
        {
            SpotifyMetadataApiClient.EnsureRateLimitIsNotExceeded = true;
            for (int i = 0; i < 100; i++)
            {
                new SpotifyMetadataApiClient().LookupArtist("spotify:artist:4YrKBkKSVeqDamzBPWVnSJ");
            }
        }
        [Test]
        public void TestLookupArtist()
        {
            var result = new SpotifyMetadataApiClient().LookupArtist("spotify:artist:4YrKBkKSVeqDamzBPWVnSJ");
            result.Should().NotBeNull();
            result.Href.Should().Be("spotify:artist:4YrKBkKSVeqDamzBPWVnSJ");
            result.Name.Should().Be("Basement Jaxx");
            result.Albums.Should().NotBeNull();
            result.Albums.Should().BeEmpty();
            result.Popularity.Should().BeNull();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestLookupArtist_Null()
        {
            new SpotifyMetadataApiClient().LookupArtist(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestLookupArtist_MalformedUri()
        {
            new SpotifyMetadataApiClient().LookupArtist("spotify:album:asdasd");
        }

        [Test]
        [ExpectedException(Handler = "EnsureSpotifyApiExceptionIsOfTypeNotFound")]
        public void TestLookupArtist_NonexistantUri()
        {
            new SpotifyMetadataApiClient().LookupArtist("spotify:artist:123");
        }


        [Test]
        public void TestLookupAlbum()
        {
            var result = new SpotifyMetadataApiClient().LookupAlbum("spotify:album:6G9fHYDCoyEErUkHrFYfs4");
            result.Should().NotBeNull();
            result.ArtistId.Should().Be("spotify:artist:4YrKBkKSVeqDamzBPWVnSJ");
            result.Artist.Should().Be("Basement Jaxx");
            result.Artists.Should().NotBeNull();
            result.Artists.Should().BeEmpty();
            result.Availability.Should().NotBeNull();
            result.Availability.Territories.Should().NotBeNullOrWhiteSpace();
            result.ExternalIds.Should().NotBeNullOrEmpty();
            result.ExternalIds.First().Id.Should().NotBeNullOrEmpty();
            result.ExternalIds.First().Type.Should().NotBeNullOrEmpty();
            result.Href.Should().Be("spotify:album:6G9fHYDCoyEErUkHrFYfs4");
            result.Name.Should().Be("Remedy");
            result.Popularity.Should().BeNullOrEmpty();
            result.Released.Should().Be("1999");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestLookupAlbum_Null()
        {
            new SpotifyMetadataApiClient().LookupAlbum(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestLookupAlbum_MalformedUri()
        {
            new SpotifyMetadataApiClient().LookupAlbum("spotify:track:asdasd");
        }

        [Test]
        [ExpectedException(Handler = "EnsureSpotifyApiExceptionIsOfTypeNotFound")]
        public void TestLookupAlbum_NonexistantUri()
        {
            new SpotifyMetadataApiClient().LookupAlbum("spotify:album:123");
        }


        [Test]
        public void TestLookupTrack()
        {
            var result = new SpotifyMetadataApiClient().LookupTrack("spotify:track:6NmXV4o6bmp704aPGyTVVG");
            result.Should().NotBeNull();
            result.Album.Should().NotBeNull();
            result.Album.ArtistId.Should().BeNull();
            result.Album.Artist.Should().BeNull();
            result.Album.Artists.Should().NotBeNull();
            result.Album.Artists.Should().BeEmpty();
            result.Album.Availability.Should().BeNull();
            result.Album.ExternalIds.Should().NotBeNull();
            result.Album.ExternalIds.Should().BeEmpty();
            result.Album.Href.Should().Be("spotify:album:6K8NUknbPh5TGaKeZdDwSg");
            result.Album.Name.Should().Be("Mann Mot Mann (Ep)");
            result.Album.Popularity.Should().BeNull();
            result.Album.Released.Should().Be("2002");
            result.Artists.Should().NotBeNullOrEmpty();
            result.Artists.First().Href.Should().Be("spotify:artist:1s1DnVoBDfp3jxjjew8cBR");
            result.Artists.First().Name.Should().Be("Kaizers Orchestra");
            result.Available.Should().BeTrue();
            result.Href.Should().Be("spotify:track:6NmXV4o6bmp704aPGyTVVG");
            result.Length.Should().Be((decimal)317.04);
            result.Name.Should().Be("B\u00f8n Fra Helvete (Live)");
            result.Popularity.Should().NotBeNullOrEmpty();
            result.DiscNumber.Should().BeNull();
            result.TrackNumber.Should().Be("2");
            result.Availability.Should().NotBeNull();
            result.Availability.Territories.Should().NotBeNullOrWhiteSpace();
            result.ExternalIds.Should().NotBeNullOrEmpty();
            result.ExternalIds.First().Id.Should().Be("NOPVA0203020");
            result.ExternalIds.First().Type.Should().Be("isrc");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestLookupTrack_Null()
        {
            new SpotifyMetadataApiClient().LookupTrack(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestLookupTrack_MalformedUri()
        {
            new SpotifyMetadataApiClient().LookupTrack("spotify:album:asdasd");
        }

        [Test]
        [ExpectedException(Handler = "EnsureSpotifyApiExceptionIsOfTypeNotFound")]
        public void TestLookupTrack_NonexistantUri()
        {
            new SpotifyMetadataApiClient().LookupTrack("spotify:track:123");
        }




        [Test]
        public void TestSearchArtists()
        {
            var result = new SpotifyMetadataApiClient().SearchArtists("foo");
            result.Should().NotBeNull();
            result.Info.Should().NotBeNull();
            result.Info.Limit.Should().Be(100);
            result.Info.NumResults.Should().BeGreaterOrEqualTo(30);
            result.Info.Offset.Should().Be(0);
            result.Info.Page.Should().Be(1);
            result.Info.Query.Should().Be("foo");
            result.Info.Type.Should().Be("artist");

            result.Artists.Should().NotBeNullOrEmpty();
            result.Artists.Find(o => o.Href == "spotify:artist:7jy3rLJdDQY21OgRLCZ9sD").Should().NotBeNull();
            result.Artists.Find(o => o.Href == "spotify:artist:7jy3rLJdDQY21OgRLCZ9sD").Name.Should().Be("Foo Fighters");
            result.Artists.Find(o => o.Href == "spotify:artist:7jy3rLJdDQY21OgRLCZ9sD").Albums.Should().NotBeNull();
            result.Artists.Find(o => o.Href == "spotify:artist:7jy3rLJdDQY21OgRLCZ9sD").Albums.Should().BeEmpty();
            result.Artists.Find(o => o.Href == "spotify:artist:7jy3rLJdDQY21OgRLCZ9sD").Href.Should().Be("spotify:artist:7jy3rLJdDQY21OgRLCZ9sD");
            result.Artists.Find(o => o.Href == "spotify:artist:7jy3rLJdDQY21OgRLCZ9sD").Popularity.Should().NotBeNullOrEmpty();
        }
        [Test]
        public void TestSearchArtists_Page2()
        {
            var result = new SpotifyMetadataApiClient().SearchArtists("foo", 40);
            result.Should().NotBeNull();
            result.Info.Should().NotBeNull();
            result.Info.Limit.Should().Be(100);
            result.Info.NumResults.Should().BeGreaterOrEqualTo(30);
            result.Info.Offset.Should().Be(3900);
            result.Info.Page.Should().Be(40);
            result.Info.Query.Should().Be("foo");
            result.Info.Type.Should().Be("artist");

            result.Artists.Should().NotBeNull();
            result.Artists.Should().BeEmpty();
        }
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSearchArtists_Page0()
        {
            new SpotifyMetadataApiClient().SearchArtists("foo", 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSearchArtists_Null()
        {
            new SpotifyMetadataApiClient().SearchArtists(null);
        }




        [Test]
        public void TestSearchAlbums()
        {
            var result = new SpotifyMetadataApiClient().SearchAlbums("foo");
            result.Should().NotBeNull();
            result.Info.Should().NotBeNull();
            result.Info.Limit.Should().Be(100);
            result.Info.NumResults.Should().BeGreaterOrEqualTo(30);
            result.Info.Offset.Should().Be(0);
            result.Info.Page.Should().Be(1);
            result.Info.Query.Should().Be("foo");
            result.Info.Type.Should().Be("album");

            result.Albums.Should().NotBeNullOrEmpty();
            result.Albums.Find(o => o.Href == "spotify:album:1zCNrbPpz5OLSr6mSpPdKm").Should().NotBeNull();
            result.Albums.Find(o => o.Href == "spotify:album:1zCNrbPpz5OLSr6mSpPdKm").Name.Should().Be("Greatest Hits");
            result.Albums.Find(o => o.Href == "spotify:album:1zCNrbPpz5OLSr6mSpPdKm").Artist.Should().BeNull();
            result.Albums.Find(o => o.Href == "spotify:album:1zCNrbPpz5OLSr6mSpPdKm").ArtistId.Should().BeNull();
            result.Albums.Find(o => o.Href == "spotify:album:1zCNrbPpz5OLSr6mSpPdKm").Artists.Should().NotBeNullOrEmpty();
            result.Albums.Find(o => o.Href == "spotify:album:1zCNrbPpz5OLSr6mSpPdKm").Availability.Should().NotBeNull();
            result.Albums.Find(o => o.Href == "spotify:album:1zCNrbPpz5OLSr6mSpPdKm").Availability.Territories.Should().NotBeNullOrEmpty();
            result.Albums.Find(o => o.Href == "spotify:album:1zCNrbPpz5OLSr6mSpPdKm").ExternalIds.Should().NotBeNullOrEmpty();
            result.Albums.Find(o => o.Href == "spotify:album:1zCNrbPpz5OLSr6mSpPdKm").ExternalIds.First().Id.Should().NotBeNullOrEmpty();
            result.Albums.Find(o => o.Href == "spotify:album:1zCNrbPpz5OLSr6mSpPdKm").ExternalIds.First().Type.Should().NotBeNullOrEmpty();
            result.Albums.Find(o => o.Href == "spotify:album:1zCNrbPpz5OLSr6mSpPdKm").Released.Should().BeNull();
            result.Albums.Find(o => o.Href == "spotify:album:1zCNrbPpz5OLSr6mSpPdKm").Href.Should().Be("spotify:album:1zCNrbPpz5OLSr6mSpPdKm");
            result.Albums.Find(o => o.Href == "spotify:album:1zCNrbPpz5OLSr6mSpPdKm").Popularity.Should().NotBeNullOrEmpty();
        }
        [Test]
        public void TestSearchAlbums_Page2()
        {
            var result = new SpotifyMetadataApiClient().SearchAlbums("foo", 40);
            result.Should().NotBeNull();
            result.Info.Should().NotBeNull();
            result.Info.Limit.Should().Be(100);
            result.Info.NumResults.Should().BeGreaterOrEqualTo(30);
            result.Info.Offset.Should().Be(3900);
            result.Info.Page.Should().Be(40);
            result.Info.Query.Should().Be("foo");
            result.Info.Type.Should().Be("album");

            result.Albums.Should().NotBeNull();
            result.Albums.Should().BeEmpty();
        }
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSearchAlbums_Page0()
        {
            new SpotifyMetadataApiClient().SearchAlbums("foo", 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSearchAlbums_Null()
        {
            new SpotifyMetadataApiClient().SearchAlbums(null);
        }




        [Test]
        public void TestSearchTracks()
        {
            var result = new SpotifyMetadataApiClient().SearchTracks("foo");
            result.Should().NotBeNull();
            result.Info.Should().NotBeNull();
            result.Info.Limit.Should().Be(100);
            result.Info.NumResults.Should().BeGreaterOrEqualTo(30);
            result.Info.Offset.Should().Be(0);
            result.Info.Page.Should().Be(1);
            result.Info.Query.Should().Be("foo");
            result.Info.Type.Should().Be("track");

            result.Tracks.Should().NotBeNullOrEmpty();
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Should().NotBeNull();
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Name.Should().Be("Everlong");
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Album.Should().NotBeNull();
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Album.Artist.Should().BeNull();
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Album.ArtistId.Should().BeNull();
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Album.Artists.Should().BeEmpty();
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Album.Availability.Should().NotBeNull();
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Album.Availability.Territories.Should().NotBeNullOrEmpty();
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Album.ExternalIds.Should().BeEmpty();
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Album.Href.Should().Be("spotify:album:1zCNrbPpz5OLSr6mSpPdKm");
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Album.Name.Should().Be("Greatest Hits");
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Album.Popularity.Should().BeNull();
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Album.Released.Should().Be("2009");
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Artists.Should().NotBeNullOrEmpty();
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Availability.Should().BeNull();
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").ExternalIds.Should().NotBeNullOrEmpty();
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").ExternalIds.First().Id.Should().NotBeNullOrEmpty();
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").ExternalIds.First().Type.Should().NotBeNullOrEmpty();
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Available.Should().NotHaveValue();
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").DiscNumber.Should().BeNull();
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Length.Should().Be((decimal)249.986);
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").TrackNumber.Should().Be("3");
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Href.Should().Be("spotify:track:07q6QTQXyPRCf7GbLakRPr");
            result.Tracks.Find(o => o.Href == "spotify:track:07q6QTQXyPRCf7GbLakRPr").Popularity.Should().NotBeNullOrEmpty();
        }
        [Test]
        public void TestSearchTracks_Page2()
        {
            var result = new SpotifyMetadataApiClient().SearchTracks("foo", 400);
            result.Should().NotBeNull();
            result.Info.Should().NotBeNull();
            result.Info.Limit.Should().Be(100);
            result.Info.NumResults.Should().BeGreaterOrEqualTo(30);
            result.Info.Offset.Should().Be(39900);
            result.Info.Page.Should().Be(400);
            result.Info.Query.Should().Be("foo");
            result.Info.Type.Should().Be("track");

            result.Tracks.Should().NotBeNull();
            result.Tracks.Should().BeEmpty();
        }
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSearchTracks_Page0()
        {
            new SpotifyMetadataApiClient().SearchTracks("foo", 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSearchTracks_Null()
        {
            new SpotifyMetadataApiClient().SearchTracks(null);
        }

        #region Helper methods

        public void EnsureSpotifyApiExceptionIsOfTypeNotFound(Exception ex)
        {
            if (!(ex is SpotifyApiException)
                || ((SpotifyApiException)ex).Type != SpotifyApiExceptionType.NotFound)
            {
                throw new Exception();
            }
        }

        #endregion
    }
}
