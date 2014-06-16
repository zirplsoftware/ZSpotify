using System;

namespace Zirpl.Spotify.MetadataAPI
{
    public class SpotifyApiException : Exception
    {
        public SpotifyApiException()
        {
        }

        public SpotifyApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public SpotifyApiException(string message)
            : base(message)
        {
        }

        public SpotifyApiException(SpotifyApiExceptionType type)
        {
            Type = type;
        }

        public SpotifyApiException(SpotifyApiExceptionType type, string message, Exception innerException)
            : base(message, innerException)
        {
            Type = type;
        }

        public SpotifyApiException(SpotifyApiExceptionType type, string message)
            : base(message)
        {
            Type = type;
        }

        public SpotifyApiExceptionType Type { get; set; }
    }

    public enum SpotifyApiExceptionType
    {
        NotModified,
        BadRequest,
        SpotifyRateLimitingInEffect,
        NotFound,
        SpotifyInternalServerError,
        SpotifyServiceUnavailable,
        Unknown,
        NetworkError
    }
}