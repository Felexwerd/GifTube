# Changelog

## 2.2.6
- Fixed infinite-scroll trigger to listen to the scrollable GIF dialog.
- Uses the API's next-page metadata when available.
- Raises the client-side result ceiling to 1,000,000; actual availability depends on GifSnap.
- Stops pagination when no new unique results are returned.

## 2.2.5
- Removed the quick-category strip from the GIF picker.
- Added paginated loading (up to 50 items per request), subject to API pagination.

## 2.2.4
- Added invisible Unicode payload format for GIF references.
- Retained parsing support for earlier GifTube comment formats.

## 2.2.2
- Improved stale-search response handling and API error feedback.
- Improved GIF preview fallback and comment-editor insertion behavior.
- Added Escape-to-close behavior.

## 2.2.1
- Search, categories, favorites, and GIF rendering baseline.
