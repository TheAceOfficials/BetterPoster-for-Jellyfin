using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.BtttrPosters.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        /// <summary>
        /// Show trend tags (Trending, New, IMDb rank). Default: true (omit tag=none query param).
        /// </summary>
        public bool TrendTags { get; set; } = true;

        /// <summary>
        /// Show quality badges (4K, Dolby Vision, Atmos). Default: false.
        /// </summary>
        public bool QualityTags { get; set; } = false;

        /// <summary>
        /// Show genre label at bottom. Default: true.
        /// </summary>
        public bool ShowGenre { get; set; } = true;

        /// <summary>
        /// Show star rating at bottom. Default: true.
        /// </summary>
        public bool ShowRating { get; set; } = true;

        /// <summary>
        /// Show age rating (PG-13, TV-MA, R). Default: false.
        /// </summary>
        public bool AgeRating { get; set; } = false;

        /// <summary>
        /// Rating source: avg, IM, TM, RT, MC, TR, LB, RE. Default: avg (omit rs query param).
        /// </summary>
        public string RatingSource { get; set; } = "avg";

        /// <summary>
        /// Poster language (ISO code). Default: en (omit lang query param).
        /// </summary>
        public string Language { get; set; } = "en";
    }
}
