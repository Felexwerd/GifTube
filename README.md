# GifTube

<p align="center">
  <strong>GIF reactions for YouTube comments</strong><br>
  <em>Created by Felexwerd · Chrome / Edge · Manifest V3</em>
</p>

<p align="center">
  <a href="#english">English</a> · <a href="#русский">Русский</a>
</p>

---

<a id="english"></a>
## 🇬🇧 English

**GifTube** is a browser extension by **Felexwerd** that lets you search for animated GIFs and use them in YouTube comment discussions. It includes GIF search, a favorites list, and an inline renderer for GifTube-formatted comments.

### Features
- Search animated GIFs through the GifSnap service.
- Browse results with automatic page loading while scrolling.
- Save and revisit favorite GIFs.
- Insert a compact `GIF` marker into a YouTube comment.
- Render compatible GifTube comments as animations for viewers who have the extension installed.
- Supports Chrome and Microsoft Edge (Manifest V3).

### Important compatibility note
GifTube's comment format stores the GIF address in an invisible Unicode payload alongside the visible `GIF` marker. **The animation is rendered by GifTube for users who have a compatible version of the extension.** Viewers without the extension may see the marker but will not see the animation. This is not a native YouTube GIF attachment.

YouTube has also been rejecting native GIF comment submissions from desktop browsers with `INVALID_ARGUMENT` (reported on 14 October 2026). GifTube does not bypass YouTube authentication and does not read cookies or access tokens.

### Install (developer/unpacked)
1. Download the ZIP from the [Releases](https://github.com/Felexwerd/GifTube/releases) page.
2. Extract the ZIP to a permanent folder.
3. Open `chrome://extensions` in Chrome or `edge://extensions` in Edge.
4. Enable **Developer mode**.
5. Choose **Load unpacked** and select the extracted folder containing `manifest.json`.
6. Open or refresh YouTube.

### Updating
For an unpacked installation, download and extract the new release, then use **Reload** on the GifTube card in the browser's extensions page (or load the new folder and remove the old entry). Browser-store installations can receive updates through the store once the extension is published there.

### GIF catalogue and pagination
The extension can request results page by page (up to 50 per API request). The configured one-million result ceiling is only a client-side safety limit; it is **not** a promise that one million unique GIFs exist or are available from GifSnap. Actual results depend on the service's catalogue, pagination metadata, and availability.

### Project links
- [Releases](https://github.com/Felexwerd/GifTube/releases)
- [Issues / bug reports](https://github.com/Felexwerd/GifTube/issues)
- [Russian guide](README_RU.md)
- [English guide](README_EN.md)
- [Release checklist](docs/RELEASING.md)

---

<a id="русский"></a>
## 🇷🇺 Русский

**GifTube** — расширение браузера от **Felexwerd** для поиска GIF-анимаций и использования их в обсуждениях YouTube. В проекте есть поиск GIF, избранное и отображение анимаций в комментариях GifTube.

### Возможности
- Поиск анимированных GIF через сервис GifSnap.
- Постраничная загрузка результатов при прокрутке вниз.
- Список избранных GIF.
- Вставка компактной метки `GIF` в комментарий YouTube.
- Отображение совместимых комментариев GifTube в виде анимации у зрителей с установленным расширением.
- Поддержка Chrome и Microsoft Edge (Manifest V3).

### Важное ограничение
Формат комментария GifTube сохраняет адрес GIF в невидимой Unicode-нагрузке рядом с видимой меткой `GIF`. **Саму анимацию показывает GifTube у пользователей с совместимой версией расширения.** Зрители без расширения могут увидеть метку, но не анимацию. Это не нативное GIF-вложение YouTube.

Также YouTube отклоняет отправку нативных GIF-комментариев с настольных браузеров с ошибкой `INVALID_ARGUMENT` (о проблеме сообщалось 14 октября 2026 года). GifTube не обходит авторизацию YouTube, не читает cookies и не получает токены доступа.

### Установка (режим разработчика)
1. Скачай ZIP со страницы [Releases](https://github.com/Felexwerd/GifTube/releases).
2. Распакуй архив в постоянную папку.
3. Открой `chrome://extensions` в Chrome или `edge://extensions` в Edge.
4. Включи **Режим разработчика**.
5. Нажми **Загрузить распакованное расширение** и выбери папку с `manifest.json`.
6. Открой YouTube или обнови страницу.

### Обновление
При установке распакованной версии скачай и распакуй новый релиз, затем нажми **Обновить** на карточке GifTube в разделе расширений (или загрузи новую папку и удали старую запись). Автоматические обновления через браузерный магазин будут доступны после публикации расширения в соответствующем магазине.

### Библиотека GIF и прокрутка
Расширение запрашивает GIF постранично (до 50 результатов за один запрос). Установленный предел в один миллион — только ограничение на стороне клиента, **это не гарантия наличия миллиона уникальных GIF**. Реальное количество зависит от каталога GifSnap, метаданных пагинации и доступности сервиса.

### Ссылки проекта
- [Релизы](https://github.com/Felexwerd/GifTube/releases)
- [Сообщить об ошибке](https://github.com/Felexwerd/GifTube/issues)
- [English guide](README_EN.md)
- [Чек-лист выпуска обновления](docs/RELEASING.md)

---

## Credits
**GifTube by Felexwerd.** GIF search is powered by the third-party GifSnap service. YouTube is a trademark of Google LLC; this is an independent community extension and is not affiliated with or endorsed by YouTube or Google.
