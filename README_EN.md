# GifTube — User Guide

**GifTube by Felexwerd** is a browser extension for searching GIFs and rendering compatible GIF comments on YouTube.

## Features
- Search GIFs through GifSnap.
- Load result pages while scrolling.
- Save GIFs to Favorites.
- Insert a visible `GIF` marker with an invisible Unicode payload.
- Render the animation for viewers who have a compatible GifTube version installed.

## Important
Viewers without GifTube will not see the animation; they may only see the visible comment marker. This is not a native YouTube GIF attachment. As of 14 October 2026, desktop YouTube native GIF comment submissions were reported to fail with `INVALID_ARGUMENT`. The extension does not read cookies or access tokens and does not bypass YouTube authentication.

The 1,000,000 limit is a client-side ceiling, not a guarantee that one million GIFs exist. GifSnap returns results in pages of up to 50 items per request.

## Install on Chrome / Edge
1. Download the latest ZIP from [Releases](https://github.com/Felexwerd/GifTube/releases).
2. Extract it to a permanent folder.
3. Open `chrome://extensions` or `edge://extensions`.
4. Enable **Developer mode**.
5. Click **Load unpacked**.
6. Select the extracted folder containing `manifest.json`.
7. Refresh YouTube.

## Updating
1. Download the ZIP for the latest release.
2. Extract it to a new folder.
3. Open the browser extensions page.
4. Remove the old GifTube entry or load the new folder using **Load unpacked**.
5. Confirm that the version shown on the extension card is current.

Unpacked extensions do not receive silent automatic updates like store-installed extensions. Automatic browser updates require publication in Chrome Web Store / Edge Add-ons.

## Report a bug
Open [Issues](https://github.com/Felexwerd/GifTube/issues) and include the GifTube version, browser, and reproduction steps. Never attach passwords, cookies, access tokens, or private data.

## Publishing a new version
See [docs/RELEASING.md](docs/RELEASING.md).

**Project author: Felexwerd.**
