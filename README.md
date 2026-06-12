# Btttr Posters - Jellyfin Plugin

![Cover Art](https://raw.githubusercontent.com/TheAceOfficials/BetterPoster-for-Jellyfin/main/cover.PNG)

Automatically fetches and applies high-quality custom posters with overlays from [btttr.cc](https://btttr.cc) for your Jellyfin media library. 

This plugin uses the IMDb ID of your movies and TV shows to find matching posters on Btttr and sets them as the primary image in Jellyfin.

## Features
- 🚀 **Automatic fetching:** Pulls custom posters directly from Btttr.cc.
- 🎯 **IMDb ID matching:** Ensures accurate poster matching for your media.
- 🖼️ **Primary Image Provider:** Integrates seamlessly as a metadata image provider in Jellyfin.

## Installation

You can easily install this plugin by adding the custom repository to your Jellyfin server.

### Step 1: Add the Repository
1. Open your Jellyfin Web UI.
2. Go to **Dashboard** > **Plugins** (under the Advanced section).
3. Click on the **Repositories** tab.
4. Click the **+** (Add) button.
5. Enter the following details:
   - **Repository Name:** Btttr Posters
   - **Repository URL:** `https://raw.githubusercontent.com/TheAceOfficials/BetterPoster-for-Jellyfin/refs/heads/main/manifest.json`
6. Click **Save**.

### Step 2: Install the Plugin
1. Go back to the **Catalog** tab in the Plugins page.
2. Scroll to find **Btttr Posters** under the *Metadata* category.
3. Click on it and select **Install**.
4. Confirm the installation.

  ![Guide](https://raw.githubusercontent.com/TheAceOfficials/BetterPoster-for-Jellyfin/main/setup.JPG)

### Step 3: Restart Jellyfin
For the plugin to take effect, you must restart your Jellyfin server.
- **Windows:** Right-click the Jellyfin tray icon and select "Restart", or restart the Jellyfin service from the Services app.
- **Linux/Docker:** Restart the Docker container or the systemd service (`sudo systemctl restart jellyfin`).
- **macOS:** Restart the Jellyfin application.

## How to Configure & Use

### Enable for Libraries

Once installed and the server is restarted, you need to enable the plugin for your libraries so it can fetch the posters.

1. Go to **Dashboard** > **Libraries**.
2. Click on the three dots `...` on a library (e.g., Movies or TV Shows) and select **Manage Library**.
3. Scroll down to the **Image fetchers** section.
4. Check the box next to **Btttr Posters**.
5. *(Highly Recommended)* Move it to the **top** of the list using the arrows so it takes priority over other image providers like TMDb or OMDB.
6. Click **Save** at the bottom.

Now, when you add new media or manually choose to **Refresh Metadata** (by selecting "Replace existing images"), Jellyfin will look for posters from Btttr.cc and apply them automatically!


### Plugin Settings

Configure poster options (optional):

1. Go to **Dashboard** > **Plugins**.
2. Under **Installed**, click **Btttr Posters**.
3. Click **Settings** and update the configuration options (trend tags, ratings, language, etc.). See [btttr.cc/configure](https://btttr.cc/configure) for details on each option.
4. Click **Save**.

## Troubleshooting
- **No poster applied?** Make sure your media has a valid IMDb ID in its metadata. The plugin relies entirely on the IMDb ID (`ttXXXXXXX`) to fetch the correct poster from Btttr.cc. You can check this by editing the metadata of your movie/show.
