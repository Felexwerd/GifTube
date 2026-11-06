// GifTube 2.2.7 — Felexwerd
// Public GifSnap API. No API key or user credentials are used.
const API_BASE = "https://gifsnap.com/api/v1/gifs";
const REQUEST_TIMEOUT_MS = 15000;

function safeHttpsUrl(value) {
  try {
    const url = new URL(String(value || ""));
    return url.protocol === "https:" && !url.username && !url.password ? url.href : "";
  } catch { return ""; }
}
function normalizeGif(gif) {
  if (!gif || typeof gif !== "object") return null;
  const full = safeHttpsUrl(gif.url);
  if (!full) return null;
  const animated = gif.animated_preview;
  const animatedMime = String(animated?.mime_type || "").toLowerCase();
  const animatedUrl = safeHttpsUrl(animated?.url);
  const animatedImagePreview = animatedUrl && animated?.animated === true &&
    ["image/gif","image/webp","image/apng"].includes(animatedMime);
  return {
    id: String(gif.id || gif.content_id || full),
    title: String(gif.title || "GIF").slice(0,160),
    preview: animatedImagePreview ? animatedUrl : full,
    full,
    mimeType: animatedImagePreview ? animatedMime : String(gif.mime_type || "")
  };
}
chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
  if (message?.type !== "GIF_TUBE_SEARCH") return;
  const mode = message.mode === "search" ? "search" : "trending";
  const page = Math.min(Math.max(Number.parseInt(message.page,10)||1,1),20000);
  const limit = Math.min(Math.max(Number.parseInt(message.limit,10)||50,1),50);
  const params = new URLSearchParams({page:String(page),limit:String(limit)});
  if (mode === "search") {
    const query = String(message.query || "").trim().slice(0,50);
    if (!query) { sendResponse({ok:true,data:[]}); return; }
    params.set("q",query);
  }
  const endpoint = mode === "trending" ? "trending" : "search";
  const controller = new AbortController();
  const timeout = setTimeout(()=>controller.abort(),REQUEST_TIMEOUT_MS);
  fetch(`${API_BASE}/${endpoint}?${params.toString()}`,{
    method:"GET",headers:{"Accept":"application/json"},signal:controller.signal
  }).then(async response=>{
    if (!response.ok) {
      if(response.status===429) throw new Error("Сервис временно ограничил запросы. Попробуй позже.");
      if(response.status===503) throw new Error("GifSnap временно недоступен. Попробуй позже.");
      throw new Error(`GIF-сервис вернул ошибку ${response.status}.`);
    }
    return response.json();
  }).then(json=>{
    if(!json || !Array.isArray(json.data)) throw new Error("GIF-сервис вернул неожиданный ответ.");
    const data=json.data.map(normalizeGif).filter(Boolean);
    const p=json.pagination && typeof json.pagination==="object" ? json.pagination : {};
    const hasNext=p.has_next===true && page<20000;
    const pagination={
      page:Number(p.page)||page,limit:Number(p.limit)||limit,
      total:p.total!==null&&p.total!==undefined&&Number.isFinite(Number(p.total))?Number(p.total):null,
      has_next:hasNext,next_page:hasNext?(Number(p.next_page)||page+1):null
    };
    sendResponse({ok:true,data,pagination});
  }).catch(error=>{
    sendResponse({ok:false,error:error?.name==="AbortError"?"Превышено время ожидания ответа GifSnap. Попробуй ещё раз.":(error?.message||"Не удалось загрузить GIF.")});
  }).finally(()=>clearTimeout(timeout));
  return true;
});

// Fetch the public update manifest from the extension service worker. Keeping
// this request here avoids page-origin CORS restrictions in YouTube content scripts.
async function fetchUpdateManifest(message) {
  const target = new URL(String(message.manifestUrl || ""));
  if (target.protocol !== "https:" || target.hostname !== "raw.githubusercontent.com" ||
      target.pathname !== "/Felexwerd/GifTube/main/update.json") {
    throw new Error("Недопустимый адрес манифеста обновлений.");
  }
  const response = await fetch(`${target.href}?t=${Number(message.cacheBust) || Date.now()}`, {
    method: "GET",
    headers: { "Accept": "application/json" },
    cache: "no-store"
  });
  if (!response.ok) throw new Error(`Update manifest HTTP ${response.status}`);
  const update = await response.json();
  if (!update || !/^\d+\.\d+\.\d+$/.test(String(update.latest_version || ""))) {
    throw new Error("Некорректный манифест обновлений.");
  }
  return update;
}

chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
  if (message?.type !== "GIF_TUBE_CHECK_UPDATE") return;
  fetchUpdateManifest(message)
    .then(update => sendResponse({ ok: true, update }))
    .catch(error => sendResponse({ ok: false, error: error?.message || "Update check failed" }));
  return true;
});
