# GifTube — Felexwerd

**GifTube 2.2.7** is a Chrome/Edge extension that adds GIF search, favorites, infinite loading, and animated GIF rendering in YouTube comments for viewers who use GifTube.

## Features
- Search and browse GIFs through GifSnap.
- Favorites stored locally in the browser.
- Infinite scrolling, up to 50 GIFs per API request and a client-side ceiling of 1,000,000 results when the source provides them.
- Compatible invisible payload for GIF links, with support for older GifTube comment markers.
- Update checks against the public `update.json` manifest and an in-page notification when a newer release is available.

## Install
1. Download the ZIP from [GitHub Releases](https://github.com/Felexwerd/GifTube/releases/latest).
2. Extract it.
3. Open `chrome://extensions/` or `edge://extensions/`.
4. Enable Developer mode.
5. Choose **Load unpacked** and select the extracted folder containing `manifest.json`.
6. Reload YouTube.

## Updating
GifTube checks for a newer version when YouTube opens and approximately every six hours. The banner's Download button opens the latest GitHub Release. The notice does not silently replace an unpacked extension; download the ZIP, replace the folder contents, and reload the extension. Browser-managed automatic updates require publishing through Chrome Web Store or Edge Add-ons.

The repository owner must attach the versioned ZIP to each GitHub Release for the download link to provide the package.

## Known YouTube limitation
Native GIF attachments are controlled by YouTube. GifTube uses a compatible text payload to preserve the GIF URL. Animated rendering is available to viewers with a compatible GifTube version; viewers without the extension may only see the visible GIF label.

## Project files
- [Russian guide](README_RU.md)
- [English guide](README_EN.md)
- [Changelog](CHANGELOG.md)
- [Release guide](docs/RELEASING.md)
- [Update manifest](update.json)

Created by **Felexwerd**.
