using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.BtttrPosters.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string PosterLanguage { get; set; } = "en";
        public bool FallbackToTmdbText { get; set; } = true;
    }
}
