using Jellyfin.Plugin.BtttrPosters;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.BtttrPosters
{
    /// <summary>
    /// Registers the BtttrPosters services with Jellyfin's DI container.
    /// </summary>
    public class PluginServiceRegistrator : IPluginServiceRegistrator
    {
        /// <inheritdoc />
        public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
        {
            // Register the image provider so Jellyfin discovers it as an IRemoteImageProvider
            serviceCollection.AddScoped<IRemoteImageProvider, BtttrImageProvider>();

            // Register a named HTTP client for the provider so GetImageResponse works correctly
            serviceCollection.AddHttpClient(BtttrImageProvider.ClientName);
        }
    }
}
