# Release checklist (GifTube)

Use this checklist whenever you publish a new version. Keep the repository and ZIP version numbers in sync.

## 1. Update the version
1. Open `manifest.json`.
2. Increase the `version` value (for example, `2.2.6` → `2.2.7`).
3. Update the version heading and changes in `CHANGELOG.md`.
4. Update version references in the README or package notes if they are explicitly versioned.

Use a new version for every public release. Do not reuse an already published version number.

## 2. Test before packaging
- Check JavaScript syntax for every `.js` file.
- Validate `manifest.json`.
- Load the unpacked extension in Chrome and Edge.
- Test opening the picker, search, favorites, scrolling/pagination, GIF insertion, and rendering an existing GifTube comment.
- Check the browser console for errors.
- Confirm the extension still explains the limitation for viewers without GifTube.

## 3. Build the ZIP
1. Create a folder named `GifTube-Felexwerd-vX.Y.Z`.
2. Put the extension files inside it: `manifest.json`, `content.js`, `background.js`, `style.css`, and `icons/icon.svg`.
3. Include the user guide if desired.
4. ZIP the folder itself, so the archive contains one top-level folder.
5. Name the archive `GifTube-Felexwerd-vX.Y.Z.zip`.
6. Extract the ZIP into a temporary folder and verify that `manifest.json` is directly inside the extracted top-level project folder.

## 4. Publish on GitHub
1. Push the updated source files to the `main` branch.
2. Open the repository's **Releases** page.
3. Choose **Draft a new release**.
4. Create a tag such as `v2.2.7` (the tag must match the manifest version).
5. Set the release title to `GifTube 2.2.7`.
6. Paste the changes from `CHANGELOG.md` into the release notes.
7. Attach `GifTube-Felexwerd-v2.2.7.zip` as a release asset.
8. Publish the release and test the download link in a private/incognito window.

## 5. Tell users how to update
For unpacked installs, users must download the new ZIP and reload/reinstall the unpacked folder. Browser-store installations can update automatically only after GifTube is published in the relevant official store and the store accepts the update.

## Safety and trust
- Never request or store a user's YouTube password, cookies, or access tokens.
- Do not promise native GIF visibility for viewers without the extension unless YouTube officially supports it.
- Do not claim a million GIFs are guaranteed; the service controls actual result availability.
- Keep the project author as **Felexwerd** and credit third-party services accurately.
