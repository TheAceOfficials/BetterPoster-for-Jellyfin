using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.BtttrPosters.Configuration
{
    public enum PosterRatingSource
    {
        Average,
        Imdb,
        Tmdb,
        RottenTomatoes,
        Metacritic,
        Trakt,
        Letterboxd,
        RogerEbert
    }

    public enum PosterLanguage
    {
        English,
        Spanish,
        French,
        German,
        PortugueseBrazil,
        PortuguesePortugal,
        Italian,
        Dutch,
        Polish,
        Russian,
        Turkish,
        Arabic,
        Japanese,
        Korean,
        Chinese,
        Hindi,
        Swedish,
        Czech
    }

    public enum PosterEngineMode
    {
        BtttrCc,
        NativeDynamic
    }

    public class PluginConfiguration : BasePluginConfiguration
    {
        // Engine Selection
        public PosterEngineMode EngineMode { get; set; } = PosterEngineMode.BtttrCc;

        // API Keys for Native Engine
        public string TmdbApiKey { get; set; } = string.Empty;
        public string FanartTvApiKey { get; set; } = string.Empty;

        // Dynamic Rotation
        public bool EnableDynamicRotation { get; set; } = false;
        public int RotationIntervalDays { get; set; } = 7;

        // Btttr.cc Options (Shared)
        public bool EnableTrendTags { get; set; } = true;
        public bool EnableQualityTags { get; set; } = false;
        public bool EnableGenre { get; set; } = true;
        public bool EnableRating { get; set; } = true;
        public PosterRatingSource RatingSource { get; set; } = PosterRatingSource.Average;
        public bool EnableAgeRating { get; set; } = false;
        public PosterLanguage Language { get; set; } = PosterLanguage.English;
        
        public bool FallbackToTmdbText { get; set; } = true;
    }
}
