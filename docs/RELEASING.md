# Releasing GifTube

1. Update the version in manifest.json.
2. Update update.json with the same latest_version and a GitHub Releases URL.
3. Run syntax/JSON checks and test the extension on YouTube.
4. Package the extension folder as a ZIP.
5. Create a GitHub Release with tag matching the version (for example, v2.2.7) and attach the ZIP asset.
6. Publish the release. Existing installations check update.json on YouTube startup and approximately every six hours.

The update notice does not silently install unpacked extensions. Users must download the ZIP, replace/reload their unpacked extension, or use a store-distributed build for browser-managed updates.
