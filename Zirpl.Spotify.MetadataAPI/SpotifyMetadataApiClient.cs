using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
#if SILVERLIGHT
using System.Threading.Tasks;
#endif
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

        public virtual AlbumSearchResult SearchAlbums(String queryString, int pageNumber = 1)
        {
            return this.ExecuteSearch<AlbumSearchResult>("album", queryString, pageNumber);
        }

        public virtual ArtistSearchResult SearchArtists(String queryString, int pageNumber = 1)
        {
            return this.ExecuteSearch<ArtistSearchResult>("artist", queryString, pageNumber);
        }

        public virtual TrackSearchResult SearchTracks(String queryString, int pageNumber = 1)
        {
            return this.ExecuteSearch<TrackSearchResult>("track", queryString, pageNumber);
        }

        public virtual Artist LookupArtist(String uri, ArtistLookupExtra? extra = null)
        {
            String extraText = null;
            if (extra != null)
            {
                switch (extra.Value)
                {
                    case ArtistLookupExtra.Album:
                        extraText = "album";
                        break;
                    case ArtistLookupExtra.AlbumDetail:
                        extraText = "albumdetails";
                        break;
                }
            }
            return this.ExecuteLookup<ArtistLookupResult>("artist", uri, extraText).Artist;
        }

        public virtual Album LookupAlbum(String uri, AlbumLookupExtra? extra = null)
        {
            String extraText = null;
            if (extra != null)
            {
                switch (extra.Value)
                {
                    case AlbumLookupExtra.Track:
                        extraText = "track";
                        break;
                    case AlbumLookupExtra.TrackDetail:
                        extraText = "trackdetails";
                        break;
                }
            }
            return this.ExecuteLookup<AlbumLookupResult>("album", uri, extraText).Album;
        }

        public virtual Track LookupTrack(String uri)
        {
            return this.ExecuteLookup<TrackLookupResult>("track", uri, null).Track;
        }

        #region Helper methods

        protected virtual T ProcessResponseAndGetData<T>(IRestResponse<T> restResponse) where T : class
        {
            if (restResponse == null)
            {
                throw new SpotifyApiException(SpotifyApiExceptionType.Unknown);
            }
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

        private T ExecuteSearch<T>(String type, String queryString, int pageNumber = 1) where T : class, new()
        {
            if (String.IsNullOrEmpty(queryString))
            {
                throw new ArgumentNullException("queryString");
            }
            if (pageNumber <= 0)
            {
                throw new ArgumentOutOfRangeException("pageNumber");
            }

            var restClient = new RestClient(String.Format("http://ws.spotify.com/search/1/{0}.json", type));
            var request = new RestRequest();
            request.AddParameter("q", queryString);
            if (pageNumber != 1)
            {
                request.AddParameter("page", pageNumber);
            }

            HandleEnsuringRateLimitIsNotExceeded();
#if SILVERLIGHT
            var task = restClient.ExecuteAwait<T>(request);
            var restReponse = task.Result;
#else
            var restReponse = restClient.Execute<T>(request);
#endif
            return ProcessResponseAndGetData(restReponse);
        }

        private T ExecuteLookup<T>(String type, String uri, String extras) where T: class, new()
        {
            if (String.IsNullOrEmpty(uri))
            {
                throw new ArgumentNullException("uri");
            }
            var uriStart = String.Format("spotify:{0}:", type);
            if (!uri.ToLowerInvariant().StartsWith(uriStart)
                || uri.Length == uriStart.Length)
            {
                throw new ArgumentException(String.Format("Malformed spotify {0} uri", type), "uri");
            }

            var restClient = new RestClient("http://ws.spotify.com/lookup/1/.json");
            var request = new RestRequest();
            request.AddParameter("uri", uri);
            if (!String.IsNullOrEmpty(extras))
            {
                request.AddParameter("extras", extras);
            }

            HandleEnsuringRateLimitIsNotExceeded();

#if SILVERLIGHT
            var task = restClient.ExecuteAwait<T>(request);
            var restReponse = task.Result;
#else
            var restReponse = restClient.Execute<T>(request);
#endif
            return ProcessResponseAndGetData(restReponse);
        }

        #endregion
    }
}