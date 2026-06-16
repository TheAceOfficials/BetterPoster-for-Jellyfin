using Jellyfin.Plugin.BtttrPosters;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.BtttrPosters
{
    /// <summary>
    /// Registers BtttrPosters services with Jellyfin's DI container.
    /// Using AddSingleton exactly as the working reference implementation does.
    /// </summary>
    public class PluginServiceRegistrator : IPluginServiceRegistrator
    {
        /// <inheritdoc />
        public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
        {
            serviceCollection.AddSingleton<IRemoteImageProvider, BtttrImageProvider>();
            serviceCollection.AddSingleton<IScheduledTask, BetterPostersRefreshTask>();
        }
    }
}
