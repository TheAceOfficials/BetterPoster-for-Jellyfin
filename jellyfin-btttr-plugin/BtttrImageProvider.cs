using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.BtttrPosters.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.BtttrPosters
{
    /// <summary>
    /// Provides Btttr.cc primary images for Movies and Series.
    /// Modeled directly on the working NeurekaSoftware/Better-Posters reference implementation.
    /// </summary>
    public class BtttrImageProvider : IRemoteImageProvider, IHasOrder
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public const string ClientName = "BtttrPosters";

        public BtttrImageProvider(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc />
        public string Name => "Btttr Posters";

        /// <inheritdoc />
        public int Order => 0;

        /// <inheritdoc />
        public bool Supports(BaseItem item) => item is Movie || item is Series;

        /// <inheritdoc />
        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
            => Supports(item) ? new[] { ImageType.Primary } : System.Array.Empty<ImageType>();

        /// <inheritdoc />
        public Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!Supports(item))
                return Task.FromResult<IEnumerable<RemoteImageInfo>>(System.Array.Empty<RemoteImageInfo>());

            // Must have IMDb ID — this is what btttr.cc uses
            var imdbId = item.GetProviderId(MetadataProvider.Imdb);
            if (string.IsNullOrWhiteSpace(imdbId))
                return Task.FromResult<IEnumerable<RemoteImageInfo>>(System.Array.Empty<RemoteImageInfo>());

            var configuration = Plugin.Instance?.Configuration ?? new PluginConfiguration();
            var url = BtttrPosterUrlBuilder.Build(imdbId, configuration);
            var language = BtttrPosterUrlBuilder.GetLanguageCode(configuration.Language) ?? "en";

            return Task.FromResult<IEnumerable<RemoteImageInfo>>(new[]
            {
                new RemoteImageInfo
                {
                    ProviderName = Name,
                    Url = url,
                    ThumbnailUrl = url,
                    Type = ImageType.Primary,
                    Width = 500,
                    Height = 750,
                    Language = language
                }
            });
        }

        /// <inheritdoc />
        public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            // Use Jellyfin's default named client — same as working reference
            return _httpClientFactory.CreateClient(NamedClient.Default).GetAsync(new System.Uri(url), cancellationToken);
        }
    }
}
