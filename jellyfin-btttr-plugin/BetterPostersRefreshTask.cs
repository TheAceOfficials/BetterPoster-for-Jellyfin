using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.BtttrPosters
{
    /// <summary>
    /// Scheduled task that automatically refreshes all movie and series
    /// posters from btttr.cc using the current plugin configuration.
    /// Users can configure the interval from Jellyfin's Dashboard > Scheduled Tasks.
    /// </summary>
    public class BetterPostersRefreshTask : IScheduledTask
    {
        private readonly ILibraryManager _libraryManager;
        private readonly IProviderManager _providerManager;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger<BetterPostersRefreshTask> _logger;

        public BetterPostersRefreshTask(
            ILibraryManager libraryManager,
            IProviderManager providerManager,
            IFileSystem fileSystem,
            ILogger<BetterPostersRefreshTask> logger)
        {
            _libraryManager = libraryManager;
            _providerManager = providerManager;
            _fileSystem = fileSystem;
            _logger = logger;
        }

        public string Name => "BetterPoster - Refresh Posters";
        public string Key => "BtttrPostersRefresh";
        public string Description => "Automatically refreshes all Movie and TV Series posters from btttr.cc based on your current plugin settings and watch progress.";
        public string Category => "BetterPoster";

        /// <summary>
        /// Default trigger: every 24 hours.
        /// Users can change the interval from Dashboard > Scheduled Tasks.
        /// Supported intervals (in ticks): 10min, 15min, 30min, 1hr, 2hr, 4hr, 6hr, 12hr, 24hr.
        /// </summary>
        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return System.Array.Empty<TaskTriggerInfo>();
        }

        public async Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Btttr Posters: Starting scheduled poster refresh.");

            var items = _libraryManager.GetItemList(new InternalItemsQuery
            {
                IncludeItemTypes = new[]
                {
                    Jellyfin.Data.Enums.BaseItemKind.Movie,
                    Jellyfin.Data.Enums.BaseItemKind.Series
                },
                IsVirtualItem = false,
                Recursive = true
            });

            int total = items.Count;

            if (total == 0)
            {
                _logger.LogInformation("Btttr Posters: No Movies or Series found in library. Skipping.");
                progress?.Report(100);
                return;
            }

            _logger.LogInformation("Btttr Posters: Found {Total} items. Starting refresh...", total);

            int done = 0;

            foreach (var item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    _logger.LogDebug("Btttr Posters: Refreshing poster for [{Name}]", item.Name);

                    var refreshOptions = new MetadataRefreshOptions(new DirectoryService(_fileSystem))
                    {
                        // Only refresh images, skip metadata scraping entirely
                        MetadataRefreshMode = MetadataRefreshMode.None,
                        ImageRefreshMode = MetadataRefreshMode.FullRefresh,
                        ReplaceAllImages = true
                    };

                    await _providerManager.RefreshSingleItem(item, refreshOptions, cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Btttr Posters: Failed to refresh poster for [{Name}]", item.Name);
                }

                done++;
                progress?.Report((double)done / total * 100.0);
            }

            _logger.LogInformation("Btttr Posters: Scheduled refresh complete. {Done}/{Total} items processed.", done, total);
        }
    }
}
