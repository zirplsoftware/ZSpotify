using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using RestSharp;

namespace Zirpl.Spotify.MetadataAPI
{
    public class SpotifyMetadataApiClient
    {
        private const int rateLimitMillseconds = 1500;
            // it's actually 1s but we add 500ms buffer since the Spotify server may get it later

        private const int rateLimitCallQuantity = 10;
        private static readonly Queue<DateTime> _requestTimes;
        private static Object _requestTimesSyncRoot;

        static SpotifyMetadataApiClient()
        {
            _requestTimes = new Queue<DateTime>();
            EnsureRateLimitIsNotExceeded = true;
        }

        private static Object RequestTimesSyncRoot
        {
            get
            {
                if (_requestTimesSyncRoot == null)
                {
                    Interlocked.CompareExchange(ref _requestTimesSyncRoot, new object(), null);
                }
                return _requestTimesSyncRoot;
            }
        }

        public static bool EnsureRateLimitIsNotExceeded { get; set; }

        private static void HandleEnsuringRateLimitIsNotExceeded()
        {
            if (EnsureRateLimitIsNotExceeded)
            {
                lock (RequestTimesSyncRoot)
                {
                    // clear out any entries greater than 1 seconds ago (with 1 second extra buffer since we don't have
                    // the exact server call times for Spotify
                    //
                    DateTime rateLimitSecondsAgoDateTime = DateTime.Now.AddMilliseconds(-rateLimitMillseconds);
                    while (_requestTimes.Count > 0
                           && _requestTimes.Peek() < rateLimitSecondsAgoDateTime)
                    {
                        _requestTimes.Dequeue();
                    }
                    if (_requestTimes.Count >= rateLimitCallQuantity)
                    {
                        // 10 have occurred in the last 1.5 seconds
                        // we don't want to be the 11th which will kick in throttling.
                        // so let's be sure to wait long enough
                        // so that the last this next call is 1.5 seconds after the 1st call of the period
                        //
                        DateTime callTimeStartingThisPeriod = _requestTimes.Peek();
                        DateTime waitUntil = callTimeStartingThisPeriod.AddMilliseconds(rateLimitMillseconds);
                        TimeSpan waitFor = waitUntil.Subtract(DateTime.Now);
                        int waitForMilliseconds = Convert.ToInt32(Math.Ceiling(waitFor.TotalMilliseconds));
                        if (waitForMilliseconds > 0)
                        {
                            Console.WriteLine("Waiting {0}ms to avoid rate limit", waitForMilliseconds);
                            Thread.Sleep(waitForMilliseconds);
                        }
                    }

                    _requestTimes.Enqueue(DateTime.Now);
                }
            }
        }

        public virtual AlbumSearchResult SearchAlbums(String queryString, int pageNumber = 1)
        {
            if (String.IsNullOrEmpty(queryString))
            {
                throw new ArgumentNullException("queryString");
            }
            if (pageNumber <= 0)
            {
                throw new ArgumentOutOfRangeException("pageNumber");
            }

            var restClient = new RestClient("http://ws.spotify.com/search/1/album.json");
            var request = new RestRequest();
            request.AddParameter("q", queryString);
            if (pageNumber != 1)
            {
                request.AddParameter("page", pageNumber);
            }

            HandleEnsuringRateLimitIsNotExceeded();
            IRestResponse<AlbumSearchResult> restReponse = restClient.Execute<AlbumSearchResult>(request);
            return ProcessResponseAndGetData(restReponse);
        }

        public virtual ArtistSearchResult SearchArtists(String queryString, int pageNumber = 1)
        {
            if (String.IsNullOrEmpty(queryString))
            {
                throw new ArgumentNullException("queryString");
            }
            if (pageNumber <= 0)
            {
                throw new ArgumentOutOfRangeException("pageNumber");
            }

            var restClient = new RestClient("http://ws.spotify.com/search/1/artist.json");
            var request = new RestRequest();
            request.AddParameter("q", queryString);
            if (pageNumber != 1)
            {
                request.AddParameter("page", pageNumber);
            }

            HandleEnsuringRateLimitIsNotExceeded();
            IRestResponse<ArtistSearchResult> restReponse = restClient.Execute<ArtistSearchResult>(request);
            return ProcessResponseAndGetData(restReponse);
        }

        public virtual TrackSearchResult SearchTracks(String queryString, int pageNumber = 1)
        {
            if (String.IsNullOrEmpty(queryString))
            {
                throw new ArgumentNullException("queryString");
            }
            if (pageNumber <= 0)
            {
                throw new ArgumentOutOfRangeException("pageNumber");
            }

            var restClient = new RestClient("http://ws.spotify.com/search/1/track.json");
            var request = new RestRequest();
            request.AddParameter("q", queryString);
            if (pageNumber != 1)
            {
                request.AddParameter("page", pageNumber);
            }

            HandleEnsuringRateLimitIsNotExceeded();
            IRestResponse<TrackSearchResult> restReponse = restClient.Execute<TrackSearchResult>(request);
            return ProcessResponseAndGetData(restReponse);
        }

        public virtual Artist LookupArtist(String uri, ArtistLookupExtra? extra = null)
        {
            if (String.IsNullOrEmpty(uri))
            {
                throw new ArgumentNullException("uri");
            }
            if (!uri.ToLowerInvariant().StartsWith("spotify:artist:")
                || uri.Length == 15)
            {
                throw new ArgumentException("Malformed spotify artist uri", "uri");
            }

            var restClient = new RestClient("http://ws.spotify.com/lookup/1/.json");
            var request = new RestRequest();
            request.AddParameter("uri", uri);
            if (extra != null)
            {
                switch (extra.Value)
                {
                    case ArtistLookupExtra.Album:
                        request.AddParameter("extras", "album");
                        break;
                    case ArtistLookupExtra.AlbumDetail:
                        request.AddParameter("extras", "albumdetails");
                        break;
                }
            }

            HandleEnsuringRateLimitIsNotExceeded();
            IRestResponse<ArtistLookupResult> restReponse = restClient.Execute<ArtistLookupResult>(request);
            return ProcessResponseAndGetData(restReponse).Artist;
        }

        public virtual Album LookupAlbum(String uri, AlbumLookupExtra? extra = null)
        {
            if (String.IsNullOrEmpty(uri))
            {
                throw new ArgumentNullException("uri");
            }
            if (!uri.ToLowerInvariant().StartsWith("spotify:album:")
                || uri.Length == 14)
            {
                throw new ArgumentException("Malformed spotify album uri", "uri");
            }

            var restClient = new RestClient("http://ws.spotify.com/lookup/1/.json");
            var request = new RestRequest();
            request.AddParameter("uri", uri);
            if (extra != null)
            {
                switch (extra.Value)
                {
                    case AlbumLookupExtra.Track:
                        request.AddParameter("extras", "track");
                        break;
                    case AlbumLookupExtra.TrackDetail:
                        request.AddParameter("extras", "trackdetails");
                        break;
                }
            }

            HandleEnsuringRateLimitIsNotExceeded();
            IRestResponse<AlbumLookupResult> restReponse = restClient.Execute<AlbumLookupResult>(request);
            return ProcessResponseAndGetData(restReponse).Album;
        }

        public virtual Track LookupTrack(String uri)
        {
            if (String.IsNullOrEmpty(uri))
            {
                throw new ArgumentNullException("uri");
            }
            if (!uri.ToLowerInvariant().StartsWith("spotify:track:")
                || uri.Length == 14)
            {
                throw new ArgumentException("Malformed spotify track uri", "uri");
            }

            var restClient = new RestClient("http://ws.spotify.com/lookup/1/.json");
            var request = new RestRequest();
            request.AddParameter("uri", uri);

            HandleEnsuringRateLimitIsNotExceeded();
            IRestResponse<TrackLookupResult> restReponse = restClient.Execute<TrackLookupResult>(request);
            return ProcessResponseAndGetData(restReponse).Track;
        }

        protected virtual T ProcessResponseAndGetData<T>(IRestResponse<T> restResponse) where T : class
        {
            if (restResponse.ResponseStatus == ResponseStatus.Completed)
            {
                switch (restResponse.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return restResponse.Data;
                        break;
                    case HttpStatusCode.NotModified:
                        throw new SpotifyApiException(SpotifyApiExceptionType.NotModified);
                        break;
                    case HttpStatusCode.NotAcceptable:
                    case HttpStatusCode.BadRequest:
                        throw new SpotifyApiException(SpotifyApiExceptionType.BadRequest);
                        break;
                    case HttpStatusCode.Forbidden:
                        throw new SpotifyApiException(SpotifyApiExceptionType.SpotifyRateLimitingInEffect);
                        break;
                    case HttpStatusCode.NotFound:
                        // probably a malformed URL OR the uri was not found
                        throw new SpotifyApiException(SpotifyApiExceptionType.NotFound);
                        break;
                    case HttpStatusCode.InternalServerError:
                        throw new SpotifyApiException(SpotifyApiExceptionType.SpotifyInternalServerError);
                        break;
                    case HttpStatusCode.ServiceUnavailable:
                        throw new SpotifyApiException(SpotifyApiExceptionType.SpotifyServiceUnavailable);
                        break;
                    default:
                        throw new SpotifyApiException(SpotifyApiExceptionType.Unknown);
                        break;
                }
            }
            throw new SpotifyApiException(SpotifyApiExceptionType.NetworkError);
        }
    }
}