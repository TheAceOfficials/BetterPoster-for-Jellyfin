using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.BtttrPosters
{
    /// <summary>
    /// Registers BtttrPosters services with Jellyfin's DI container.
    /// </summary>
    public class PluginServiceRegistrator : IPluginServiceRegistrator
    {
        /// <inheritdoc />
        public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
        {
            // Register image provider so Jellyfin discovers it as IRemoteImageProvider
            serviceCollection.AddScoped<IRemoteImageProvider, BtttrImageProvider>();

            // Register named HTTP client so GetImageResponse works correctly
            serviceCollection.AddHttpClient(BtttrImageProvider.ClientName);
        }
    }
}
