using System;
using System.Collections.Generic;
using System.IO;
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

        public BtttrImageProvider(IHttpClientFactory httpClientFactory, ILogger<BtttrImageProvider> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public string Name => "Btttr Posters";

        public int Order => 0; // Highest priority - displays as the first choice

        public bool Supports(BaseItem item)
        {
            // Only movies and series (TV Shows) support custom poster overlays
            return item is Movie || item is Series;
        }

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[] { ImageType.Primary };
        }

        public Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
        {
            var images = new List<RemoteImageInfo>();
            
            // Extract the Identifier from Jellyfin's metadata item links
            string? imdbId = item.GetProviderId(MetadataProvider.Imdb);
            string? targetId = null;
            string idType = "imdb";

            if (!string.IsNullOrEmpty(imdbId))
            {
                targetId = imdbId;
                // Ensure IMDb ID starts with "tt" (normal IMDb format, e.g., tt10919420)
                if (!targetId.StartsWith("tt", StringComparison.OrdinalIgnoreCase))
                {
                    targetId = "tt" + targetId;
                }
            }
            else
            {
                bool fallback = Plugin.Instance?.Configuration?.FallbackToTmdbText ?? true;
                if (fallback)
                {
                    string? tmdbId = item.GetProviderId(MetadataProvider.Tmdb);
                    if (!string.IsNullOrEmpty(tmdbId))
                    {
                        targetId = tmdbId;
                        idType = "tmdb";
                    }
                }
            }

            _logger.LogInformation("Processing Btttr Image Provider for item: {Name}", item.Name);

            if (string.IsNullOrEmpty(targetId))
            {
                _logger.LogWarning("Btttr Image Provider: IMDB/TMDB ID not found for item: {Name}. Cannot fetch btttr.cc custom poster.", item.Name);
                return Task.FromResult<IEnumerable<RemoteImageInfo>>(images);
            }

            var config = Plugin.Instance?.Configuration ?? new PluginConfiguration();

            string suffix = config.EnableGenre switch
            {
                true when config.EnableRating => string.Empty,
                false when config.EnableRating => "r",
                true => "g",
                false => "n"
            };

            if (config.EnableQualityTags) suffix += "q";
            if (config.EnableAgeRating) suffix += "a";

            string path = string.IsNullOrEmpty(suffix) ? "poster" : "poster-" + suffix;
            
            string btttrUrl = $"https://btttr.cc/{path}/{idType}/poster-default/{Uri.EscapeDataString(targetId)}.jpg";

            var queryParams = new List<string>();

            if (!config.EnableTrendTags)
            {
                queryParams.Add("tag=none");
            }

            string languageCode = GetLanguageCode(config.Language);
            if (!string.IsNullOrEmpty(languageCode))
            {
                queryParams.Add($"lang={Uri.EscapeDataString(languageCode)}");
            }

            string ratingSourceCode = GetRatingSourceCode(config.RatingSource);
            if (config.EnableRating && !string.IsNullOrEmpty(ratingSourceCode))
            {
                queryParams.Add($"rs={Uri.EscapeDataString(ratingSourceCode)}");
            }

            if (queryParams.Count > 0)
            {
                btttrUrl += "?" + string.Join("&", queryParams);
            }

            _logger.LogInformation("Generating Btttr.cc URL format: {Url}", btttrUrl);

            images.Add(new RemoteImageInfo
            {
                ProviderName = Name,
                Url = btttrUrl,
                ThumbnailUrl = btttrUrl,
                Type = ImageType.Primary
            });

            return Task.FromResult<IEnumerable<RemoteImageInfo>>(images);
        }

        public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching custom poster from Btttr: {Url}", url);
            var client = _httpClientFactory.CreateClient(Name);
            return client.GetAsync(url, cancellationToken);
        }

        private static string GetLanguageCode(PosterLanguage language)
        {
            return language switch
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
        }

        private static string GetRatingSourceCode(PosterRatingSource ratingSource)
        {
            return ratingSource switch
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
}
