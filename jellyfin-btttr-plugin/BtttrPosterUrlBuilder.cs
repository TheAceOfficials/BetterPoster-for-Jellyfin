using System.Collections.Generic;
using System.Text;
using Jellyfin.Plugin.BtttrPosters.Configuration;

// Builder for creating the URL for btttr.cc posters based on the plugin configuration.
namespace Jellyfin.Plugin.BtttrPosters
{
    public static class BtttrPosterUrlBuilder
    {
        private const string BaseUrl = "https://btttr.cc";

        public static string Build(string imdbId, PluginConfiguration config)
        {
            var pathPrefix = BuildPathPrefix(config);
            var url = $"{BaseUrl}/{pathPrefix}/imdb/poster-default/{imdbId}.jpg";

            var queryParams = BuildQueryParams(config);
            if (queryParams.Count > 0)
            {
                url += "?" + string.Join("&", queryParams);
            }

            return url;
        }

        private static string BuildPathPrefix(PluginConfiguration config)
        {
            // Mirrors https://btttr.cc/configure updateAioUrl() path encoding.
            string basePath;
            if (config.ShowGenre && config.ShowRating)
            {
                basePath = "poster";
            }
            else if (config.ShowGenre)
            {
                basePath = "poster-g";
            }
            else if (config.ShowRating)
            {
                basePath = "poster-r";
            }
            else
            {
                basePath = "poster-n";
            }

            var suffix = new StringBuilder();
            if (config.QualityTags)
            {
                suffix.Append('q');
            }

            if (config.AgeRating)
            {
                suffix.Append('a');
            }

            if (suffix.Length > 0)
            {
                basePath += basePath.Contains('-', System.StringComparison.Ordinal)
                    ? suffix.ToString()
                    : "-" + suffix;
            }

            return basePath;
        }

        private static List<string> BuildQueryParams(PluginConfiguration config)
        {
            var queryParams = new List<string>();

            if (!config.TrendTags)
            {
                queryParams.Add("tag=none");
            }

            if (!string.IsNullOrEmpty(config.Language) && config.Language != "en")
            {
                queryParams.Add("lang=" + config.Language);
            }

            if (!string.IsNullOrEmpty(config.RatingSource) && config.RatingSource != "avg")
            {
                queryParams.Add("rs=" + config.RatingSource);
            }

            return queryParams;
        }
    }
}
