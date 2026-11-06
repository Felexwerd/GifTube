# GifTube 2.2.7 — Felexwerd

A YouTube GIF extension with GIF search, favorites, infinite loading, and animated GIF rendering in comments for GifTube users.

## 💜 Join the GifTube Telegram community

Follow project news, hear about new releases, share feature ideas, report bugs, and chat with other GifTube users.

👉 **[JOIN THE TELEGRAM GROUP — @youtubegifs](https://t.me/youtubegifs)**

## What's new in 2.2.7
- Version checks using update.json in this repository.
- A compact YouTube banner appears when a newer version is available.
- Download opens the GitHub Releases page; Later dismisses the notice for that version.
- Checks on YouTube startup and approximately every six hours.
- Network failures during update checks never interrupt GIF search or YouTube.

## Install
1. Download and extract the release ZIP.
2. Open chrome://extensions/ or edge://extensions/.
3. Enable Developer mode.
4. Select Load unpacked and choose the folder containing manifest.json.
5. Reload the YouTube tab.

To update an unpacked build, replace the folder contents and press Reload on the extension card, or remove the old unpacked build and load the new folder.

## Important
The notice informs users about new versions but does not silently install them. Chrome and Edge do not let a regular extension replace an unpacked installation automatically. The repository owner must attach the ZIP to each GitHub Release for the download button to provide the packaged build.

Native GIF attachments depend on YouTube. GifTube uses a compatible text payload; animated GIFs render automatically for viewers with a compatible GifTube version. Viewers without the extension may only see the GIF label.
