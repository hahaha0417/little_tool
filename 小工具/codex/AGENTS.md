# Repository Guidelines

## 專案結構與模組組織
目前這個工作區只包含工作區中繼資料：
- `codex.code-workspace`：編輯器設定
- `.github/copilot-instructions.md`：代理人指引

如果之後加入原始碼，請維持可預期的結構，例如：`src/` 放實作、`test/` 或 `tests/` 放自動化測試、`assets/` 放靜態資源。相關模組盡量放在一起，避免功能檔案散落在根目錄。

## 建置、測試與開發指令
目前尚未定義建置或測試腳本。未來加入套件或建置工具時，請在此明確記錄實際指令，並以專案宣告的入口為準，例如：
- `npm test`：執行測試
- `npm run build`：產生正式版建置
- `npm run dev`：啟動本機開發環境

新增工具時，請讓指令與 `package.json`、`Makefile` 或其他專案中繼資料保持一致。

## 程式碼風格與命名規則
請遵循實際使用的語言與格式化工具風格。若尚未定義，先依下列原則處理：
- 使用 ASCII 命名，識別字要清楚具體。
- 除非語言慣例要求，否則優先採用該生態系的標準命名方式。
- 檔名以功能或用途命名，例如 `auth-service.ts` 或 `user_card.py`。

若專案新增格式化器或 lint 工具，請把執行指令寫在此處，並在送出前先執行。

## 測試準則
請依專案採用的測試框架新增測試。測試名稱應清楚描述行為，例如 `creates_session_when_credentials_are_valid`。測試檔請放在標準測試目錄，並在可行時與來源結構保持對應。

## Commit 與 Pull Request 規範
這個工作區目前沒有 git 歷史，因此無法推斷既有 commit 慣例。請使用簡短、祈使句的 commit 訊息，例如 `add auth parser` 或 `fix workspace path`。

Pull request 應包含：
- 變更摘要
- 變更原因
- 行為有變動時附上截圖或記錄
- 如有關聯議題，請附上連結

## 代理人備註
將 `.github/copilot-instructions.md` 視為目前有效的專案專屬指引，供自動化流程與代理人行為遵循。當專案結構、工具或流程變更時，請同步更新本文件。
