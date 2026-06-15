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

    public class PluginConfiguration : BasePluginConfiguration
    {
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
