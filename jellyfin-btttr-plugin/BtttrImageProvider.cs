using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;
using Jellyfin.Plugin.BtttrPosters.Configuration;

namespace Jellyfin.Plugin.BtttrPosters
{
    public class BtttrImageProvider : IRemoteImageProvider, IHasOrder
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BtttrImageProvider> _logger;

        public const string ClientName = "BtttrPosters";

        public BtttrImageProvider(
            IHttpClientFactory httpClientFactory,
            ILogger<BtttrImageProvider> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public string Name => "Btttr Posters";
        public int Order => 0;

        public bool Supports(BaseItem item) => item is Movie || item is Series;

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
            => new[] { ImageType.Primary };

        public Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
        {
            var images = new List<RemoteImageInfo>();

            try
            {
                var config = Plugin.Instance?.Configuration ?? new PluginConfiguration();

                // --- Resolve ID ---
                string? targetId = item.GetProviderId(MetadataProvider.Imdb);
                if (!string.IsNullOrEmpty(targetId) && !targetId.StartsWith("tt", StringComparison.OrdinalIgnoreCase))
                    targetId = "tt" + targetId;

                if (string.IsNullOrEmpty(targetId) && config.FallbackToTmdbText)
                    targetId = item.GetProviderId(MetadataProvider.Tmdb);

                if (string.IsNullOrEmpty(targetId))
                {
                    _logger.LogWarning("Btttr: No IMDB/TMDB ID for [{Name}]. Skipping.", item.Name);
                    return Task.FromResult<IEnumerable<RemoteImageInfo>>(images);
                }

                // --- Media type ---
                string mediaType = item is Movie ? "movie" : "series";

                // --- Overlay config segment ---
                string overlayConfig = BuildOverlayConfig(config);

                // --- Assemble URL ---
                string btttrUrl = $"https://btttr.cc/{overlayConfig}/{mediaType}/{Uri.EscapeDataString(targetId)}/poster.jpg";

                // --- Query params ---
                var queryParams = new List<string>();

                if (!config.EnableTrendTags)
                    queryParams.Add("tag=none");

                string? languageCode = GetLanguageCode(config.Language);
                if (!string.IsNullOrEmpty(languageCode))
                    queryParams.Add($"lang={Uri.EscapeDataString(languageCode)}");

                string? ratingCode = GetRatingSourceCode(config.RatingSource);
                if (config.EnableRating && !string.IsNullOrEmpty(ratingCode))
                    queryParams.Add($"rs={Uri.EscapeDataString(ratingCode)}");

                if (queryParams.Count > 0)
                    btttrUrl += "?" + string.Join("&", queryParams);

                _logger.LogInformation("Btttr URL for [{Name}]: {Url}", item.Name, btttrUrl);

                images.Add(new RemoteImageInfo
                {
                    ProviderName = Name,
                    Url = btttrUrl,
                    ThumbnailUrl = btttrUrl,
                    Type = ImageType.Primary
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Btttr: Error building image URL for [{Name}]", item.Name);
            }

            return Task.FromResult<IEnumerable<RemoteImageInfo>>(images);
        }

        private static string BuildOverlayConfig(PluginConfiguration config)
        {
            var parts = new System.Text.StringBuilder();
            if (config.EnableGenre) parts.Append('g');
            if (config.EnableRating) parts.Append('r');
            if (config.EnableQualityTags) parts.Append('q');
            if (config.EnableAgeRating) parts.Append('a');

            return parts.Length > 0 ? "poster-" + parts : "poster";
        }

        public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Btttr: Fetching poster from {Url}", url);
            var client = _httpClientFactory.CreateClient(ClientName);
            return client.GetAsync(url, cancellationToken);
        }

        private static string? GetLanguageCode(PosterLanguage language) => language switch
        {
            PosterLanguage.English => null,
            PosterLanguage.Spanish => "es",
            PosterLanguage.French => "fr",
            PosterLanguage.German => "de",
            PosterLanguage.PortugueseBrazil => "pt-BR",
            PosterLanguage.PortuguesePortugal => "pt-PT",
            PosterLanguage.Italian => "it",
            PosterLanguage.Dutch => "nl",
            PosterLanguage.Polish => "pl",
            PosterLanguage.Russian => "ru",
            PosterLanguage.Turkish => "tr",
            PosterLanguage.Arabic => "ar",
            PosterLanguage.Japanese => "ja",
            PosterLanguage.Korean => "ko",
            PosterLanguage.Chinese => "zh",
            PosterLanguage.Hindi => "hi",
            PosterLanguage.Swedish => "sv",
            PosterLanguage.Czech => "cs",
            _ => null
        };

        private static string? GetRatingSourceCode(PosterRatingSource ratingSource) => ratingSource switch
        {
            PosterRatingSource.Average => null,
            PosterRatingSource.Imdb => "IM",
            PosterRatingSource.Tmdb => "TM",
            PosterRatingSource.RottenTomatoes => "RT",
            PosterRatingSource.Metacritic => "MC",
            PosterRatingSource.Trakt => "TR",
            PosterRatingSource.Letterboxd => "LB",
            PosterRatingSource.RogerEbert => "RE",
            _ => null
        };
    }
}
