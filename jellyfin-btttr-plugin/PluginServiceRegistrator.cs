using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.BtttrPosters
{
    /// <summary>
    /// Registers BtttrPosters services with Jellyfin's DI container.
    /// NOTE: BtttrImageProvider does NOT need to be registered here.
    /// Jellyfin auto-discovers all IRemoteImageProvider implementations
    /// from the plugin assembly via assembly scanning.
    /// Registering it here again causes a double-registration conflict
    /// which silently breaks GetImages() returning 0 results.
    /// </summary>
    public class PluginServiceRegistrator : IPluginServiceRegistrator
    {
        /// <inheritdoc />
        public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
        {
            // Only register the named HTTP client.
            // BtttrImageProvider is auto-discovered by Jellyfin — do NOT register it here.
            serviceCollection.AddHttpClient(BtttrImageProvider.ClientName);
        }
    }
}
