using System;
using System.Collections.Generic;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Jellyfin.Plugin.BtttrPosters.Configuration;

namespace Jellyfin.Plugin.BtttrPosters
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        // This name is what Jellyfin uses to look up the config page.
        // The page URL will be: /web/configurationpage?name=Btttr+Posters
        // The PluginPageInfo.Name MUST match this exactly.
        public override string Name => "Btttr Posters";

        public override Guid Id => Guid.Parse("b1ea6fb2-e42a-4632-841f-82ffba8307db");

        public static Plugin? Instance { get; private set; }

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    // CRITICAL: Name here must match Plugin.Name exactly.
                    // Do NOT append " Configuration" — that breaks the settings page load.
                    Name = Name,
                    EmbeddedResourcePath = $"{GetType().Namespace}.Configuration.configPage.html"
                }
            };
        }
    }
}
