using System.Text.Json;

namespace CppDocGenerator.Services;

public sealed class HtmlExportService
{
    public const string HtmlFileName = "index.html";
    private static readonly JsonSerializerOptions HtmlJsonOptions = new()
    {
        WriteIndented = false
    };

    public string Save(string outputFolder)
    {
        Directory.CreateDirectory(outputFolder);
        var outputPath = Path.Combine(outputFolder, HtmlFileName);
        File.WriteAllText(outputPath, BuildHtml(outputFolder));
        return outputPath;
    }

    private static string BuildHtml(string outputFolder)
    {
        var embeddedJsonMap = BuildEmbeddedJsonMap(outputFolder);

        return $$"""
<!DOCTYPE html>
<html lang="zh-Hant">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>C++ Doc</title>
  <style>
    :root {
      --bg: #0b1220;
      --bg-2: #111a2b;
      --panel: rgba(16, 24, 40, 0.9);
      --panel-2: rgba(20, 31, 52, 0.95);
      --line: #22324c;
      --text: #e8eefc;
      --muted: #93a4c3;
      --accent: #54c6eb;
      --accent-2: #ff8f5a;
      --code: #0f1727;
      --shadow: 0 18px 48px rgba(0, 0, 0, 0.32);
    }

    * { box-sizing: border-box; }
    body {
      margin: 0;
      font-family: "Segoe UI", "Noto Sans TC", sans-serif;
      color: var(--text);
      background:
        radial-gradient(circle at top left, rgba(84, 198, 235, 0.18), transparent 22%),
        radial-gradient(circle at bottom right, rgba(255, 143, 90, 0.14), transparent 24%),
        linear-gradient(135deg, var(--bg), var(--bg-2));
    }

    .app {
      display: grid;
      grid-template-columns: 320px 1fr;
      min-height: 100vh;
    }

    .sidebar, .content {
      padding: 20px;
    }

    .sidebar {
      border-right: 1px solid var(--line);
      background: linear-gradient(180deg, rgba(9, 15, 28, 0.96), rgba(15, 24, 40, 0.92));
      box-shadow: inset -1px 0 0 rgba(255,255,255,0.03);
      overflow: auto;
    }

    .content {
      overflow: auto;
    }

    h1, h2, h3 {
      margin: 0 0 12px;
    }

    .subtle {
      color: var(--muted);
      font-size: 13px;
    }

    .tree {
      margin-top: 18px;
      font-size: 14px;
    }

    .tree details {
      margin: 4px 0;
    }

    .tree summary {
      cursor: pointer;
      color: var(--text);
      padding: 4px 0;
    }

    .tree summary::marker {
      color: var(--accent);
    }

    .tree button {
      border: 0;
      background: transparent;
      cursor: pointer;
      color: var(--accent);
      text-align: left;
      padding: 4px 0 4px 18px;
      font: inherit;
    }

    .tree button:hover,
    .action:hover {
      color: var(--accent-2);
    }

    .card {
      background: var(--panel);
      border: 1px solid var(--line);
      border-radius: 16px;
      padding: 18px;
      box-shadow: var(--shadow);
      backdrop-filter: blur(10px);
      margin-bottom: 18px;
    }

    .meta {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
      gap: 12px;
      margin-top: 14px;
    }

    .meta div {
      background: var(--panel-2);
      border-radius: 12px;
      padding: 12px;
      border: 1px solid #293b58;
    }

    .meta label {
      display: block;
      color: var(--muted);
      font-size: 12px;
      margin-bottom: 6px;
      text-transform: uppercase;
      letter-spacing: 0.04em;
    }

    .symbols {
      display: grid;
      gap: 12px;
    }

    .symbol {
      background: linear-gradient(180deg, rgba(17, 26, 43, 0.96), rgba(12, 20, 35, 0.96));
      border: 1px solid var(--line);
      border-radius: 14px;
      padding: 14px;
    }

    .kind {
      display: inline-block;
      font-size: 12px;
      padding: 4px 8px;
      border-radius: 999px;
      background: rgba(84, 198, 235, 0.12);
      color: var(--accent);
      margin-bottom: 8px;
    }

    pre {
      margin: 8px 0 0;
      white-space: pre-wrap;
      word-break: break-word;
      background: var(--code);
      padding: 12px;
      border-radius: 12px;
      border: 1px solid #243552;
      font-family: Consolas, "Courier New", monospace;
      font-size: 13px;
      color: #d7e4ff;
    }

    .action {
      color: var(--accent);
      text-decoration: none;
      font-weight: 600;
      margin-right: 14px;
    }

    code {
      background: rgba(84, 198, 235, 0.1);
      color: #bfefff;
      padding: 1px 6px;
      border-radius: 6px;
      border: 1px solid rgba(84, 198, 235, 0.18);
    }

    .empty {
      color: var(--muted);
      padding: 32px 12px;
    }

    @media (max-width: 900px) {
      .app { grid-template-columns: 1fr; }
      .sidebar { border-right: 0; border-bottom: 1px solid var(--line); max-height: 42vh; }
    }
  </style>
</head>
<body>
  <div class="app">
    <aside class="sidebar">
      <h1>C++ Doc</h1>
      <div class="subtle">由樹狀 JSON 載入，顯示分析結果與 GitHub 深連結。</div>
      <div class="subtle" style="margin-top:8px">若瀏覽器直接開啟 <code>file://</code> 無法讀 JSON，請用簡單靜態伺服器開啟此資料夾。</div>
      <div id="tree" class="tree"></div>
    </aside>
    <main class="content">
      <div id="content" class="empty">請從左側選擇檔案。</div>
    </main>
  </div>

  <script>
    const embeddedDocs = {{embeddedJsonMap}};
    const treeRoot = document.getElementById("tree");
    const contentRoot = document.getElementById("content");

    function getValue(obj, camelName, pascalName = null) {
      if (!obj || typeof obj !== "object") {
        return undefined;
      }

      if (camelName in obj) {
        return obj[camelName];
      }

      const fallbackName = pascalName ?? (camelName.charAt(0).toUpperCase() + camelName.slice(1));
      return obj[fallbackName];
    }

    function getArray(obj, camelName, pascalName = null) {
      const value = getValue(obj, camelName, pascalName);
      return Array.isArray(value) ? value : [];
    }

    async function loadJson(path) {
      const normalizedPath = String(path || "").replace(/\\/g, "/");
      if (normalizedPath in embeddedDocs) {
        return embeddedDocs[normalizedPath];
      }

      if ("fetch" in window && !location.protocol.startsWith("file")) {
        const response = await fetch(normalizedPath, { cache: "no-store" });
        if (!response.ok) {
          throw new Error(`Failed to load ${normalizedPath}: ${response.status}`);
        }

        return await response.json();
      }

      throw new Error(`Embedded JSON not found: ${normalizedPath}`);
    }

    function escapeHtml(value) {
      return (value ?? "").replace(/[&<>"]/g, ch => ({
        "&": "&amp;",
        "<": "&lt;",
        ">": "&gt;",
        "\"": "&quot;"
      }[ch]));
    }

    function renderTreeNode(item) {
      const nodeType = getValue(item, "nodeType");
      const relativePath = getValue(item, "relativePath");
      const name = getValue(item, "name");
      const jsonPath = getValue(item, "jsonPath");

      if (nodeType === "folder") {
        const details = document.createElement("details");
        if (relativePath === ".") {
          details.open = true;
        }

        const summary = document.createElement("summary");
        summary.textContent = name;
        details.appendChild(summary);

        const container = document.createElement("div");
        container.style.paddingLeft = "12px";
        details.appendChild(container);

        let loaded = false;
        details.addEventListener("toggle", async () => {
          if (!details.open || loaded) {
            return;
          }

          loaded = true;
          const folderIndex = await loadJson(jsonPath);
          for (const child of getArray(folderIndex, "children")) {
            container.appendChild(renderTreeNode(child));
          }
        });

        return details;
      }

      const button = document.createElement("button");
      button.type = "button";
      button.textContent = name;
      button.addEventListener("click", () => renderFile(jsonPath));
      return button;
    }

    function flattenSymbols(symbols, depth = 0) {
      const rows = [];
      for (const symbol of (symbols ?? [])) {
        rows.push({ ...symbol, depth });
        rows.push(...flattenSymbols(getArray(symbol, "children"), depth + 1));
      }
      return rows;
    }

    async function renderFile(jsonPath) {
      const file = await loadJson(jsonPath);
      const symbols = flattenSymbols(getArray(file, "symbols"));
      const symbolHtml = symbols.length === 0
        ? '<div class="empty">這個檔案目前沒有分析到可顯示的符號。</div>'
        : `<div class="symbols">${symbols.map(symbol => `
            <section class="symbol">
              <div class="kind">${escapeHtml(getValue(symbol, "kind") ?? "")}</div>
              <h3 style="margin-left:${symbol.depth * 18}px">${escapeHtml(getValue(symbol, "qualifiedName") || getValue(symbol, "name") || "")}</h3>
              <div class="subtle">Line ${getValue(symbol, "lineNumber") ?? 0} | ${escapeHtml(getValue(symbol, "accessModifier") || "-")}</div>
              ${getValue(symbol, "summary") ? `<p>${escapeHtml(getValue(symbol, "summary"))}</p>` : ""}
              ${getValue(symbol, "signature") ? `<pre>${escapeHtml(getValue(symbol, "signature"))}</pre>` : ""}
              ${getValue(symbol, "gitHubUrl") ? `<a class="action" href="${getValue(symbol, "gitHubUrl")}" target="_blank" rel="noreferrer">Open on GitHub</a>` : ""}
            </section>
          `).join("")}</div>`;

      contentRoot.className = "";
      contentRoot.innerHTML = `
        <section class="card">
          <h2>${escapeHtml(getValue(file, "fileName") || "")}</h2>
          <div class="subtle">${escapeHtml(getValue(file, "relativePath") || "")}</div>
          <div style="margin-top:12px">
            ${getValue(file, "gitHubBlobUrl") ? `<a class="action" href="${getValue(file, "gitHubBlobUrl")}" target="_blank" rel="noreferrer">GitHub Blob</a>` : ""}
            ${getValue(file, "gitHubRawUrl") ? `<a class="action" href="${getValue(file, "gitHubRawUrl")}" target="_blank" rel="noreferrer">GitHub Raw</a>` : ""}
          </div>
          <div class="meta">
            <div><label>Extension</label><strong>${escapeHtml(getValue(file, "extension") || "")}</strong></div>
            <div><label>Language</label><strong>${escapeHtml(getValue(file, "language") || "")}</strong></div>
            <div><label>Lines</label><strong>${getValue(file, "lineCount") ?? 0}</strong></div>
            <div><label>Source Path</label><strong>${escapeHtml(getValue(file, "sourceFilePath") || "")}</strong></div>
          </div>
        </section>
        <section class="card">
          <h2>Symbols</h2>
          ${symbolHtml}
        </section>
      `;
    }

    async function boot() {
      try {
        const rootIndex = await loadJson("index.json");
        treeRoot.innerHTML = "";
        for (const child of getArray(rootIndex, "children")) {
          treeRoot.appendChild(renderTreeNode(child));
        }
      } catch (error) {
        contentRoot.className = "card";
        contentRoot.innerHTML = `
          <h2>載入失敗</h2>
          <p>${escapeHtml(String(error.message || error))}</p>
          <p class="subtle">這份 HTML 會優先使用內嵌 JSON。若仍失敗，代表輸出資料可能不完整，請重新產生 JSON 與 HTML。</p>
        `;
      }
    }

    boot();
  </script>
</body>
</html>
""";
    }

    private static string BuildEmbeddedJsonMap(string outputFolder)
    {
        var jsonFiles = Directory
            .EnumerateFiles(outputFolder, "*.json", SearchOption.AllDirectories)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                path => NormalizePath(Path.GetRelativePath(outputFolder, path)),
                path => JsonSerializer.Deserialize<object>(File.ReadAllText(path)) ?? new object(),
                StringComparer.OrdinalIgnoreCase);

        return JsonSerializer.Serialize(jsonFiles, HtmlJsonOptions);
    }

    private static string NormalizePath(string path)
    {
        return path.Replace('\\', '/');
    }
}
