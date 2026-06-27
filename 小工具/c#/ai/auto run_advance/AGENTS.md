# AGENTS.md

## 專案概述

這是一個單一專案的 Windows Forms 桌面工具，解決的問題很直接：讓使用者在 GUI 內填入要執行的指令與參數，按下執行後啟動外部程式，並可選擇在程式結束後自動重啟。

- Solution: `auto run.sln`
- Project: `auto run/auto run.csproj`
- Framework: `.NET 9`, `net9.0-windows7.0`
- UI 類型: WinForms
- 主要語言: C#

## 核心行為

應用程式啟動後會從工作目錄下的 `option.ini` 讀取設定，載入到表單欄位：

- `command`: 要執行的程式路徑或命令
- `parameter`: 啟動參數
- `check_autoload`: 是否在子程序結束後自動重啟

使用者按下 `執行` 後：

1. 建立 `Process`
2. 用 `command` + `parameter` 啟動外部程式
3. 將狀態面板切成綠色
4. 訂閱 `Exited` 事件

外部程式結束後：

1. 面板先切回紅色
2. 若 `running_ == true` 且 `check_autoload == true`，則重新啟動一次新的 `Process`

按下 `關閉` 只會把內部 `running_` 旗標改為 `false` 並把面板設為紅色，不會主動終止目前已經啟動的外部程序。

## 重要檔案

- [auto run/Program.cs](D:/vs/c#/auto%20run_advance/auto%20run/Program.cs): WinForms 進入點，啟動 `form_auto_run`
- [auto run/Form1.cs](D:/vs/c#/auto%20run_advance/auto%20run/Form1.cs): 主要業務邏輯，包含讀寫設定、啟動程序、自動重啟
- [auto run/Form1.Designer.cs](D:/vs/c#/auto%20run_advance/auto%20run/Form1.Designer.cs): 表單配置與控制項
- [auto run/ini.cs](D:/vs/c#/auto%20run_advance/auto%20run/ini.cs): 用 Win32 API 讀寫 INI 檔
- [auto run/bin/x64/Debug/net9.0-windows7.0/option.ini](D:/vs/c#/auto%20run_advance/auto%20run/bin/x64/Debug/net9.0-windows7.0/option.ini): 範例輸出目錄中的設定檔

## UI 結構

表單控制項很少，邏輯集中：

- `command`: 指令輸入框
- `parameter`: 參數輸入框
- `check_autoload`: 自動重載勾選框
- `button_run`: 啟動外部程式
- `button_close`: 停止自動重啟旗標
- `panel_autoload`: 狀態燈號，紅色表示未運行，綠色表示已啟動

## 建置與執行

### 建置

```powershell
dotnet build "auto run.sln"
```

已驗證目前可成功建置。

### 執行

```powershell
dotnet run --project ".\auto run\auto run.csproj"
```

或直接執行輸出目錄內的 `auto run.exe`。

## 設定檔格式

`option.ini` 格式如下：

```ini
[auto_run]
command=
parameter=
check_autoload=false
```

注意目前程式使用 `Directory.GetCurrentDirectory()` 組出 `option.ini` 路徑，所以設定檔實際讀寫位置取決於啟動時的工作目錄，不一定是執行檔所在目錄。

## 已確認的建置警告

`dotnet build` 目前有 3 個 warning：

- `Form1.cs`: `myProcess_Exited(object sender, EventArgs e)` 的 nullable 簽章與 `EventHandler` 不完全相符
- `Form1.Designer.cs`: 存在未使用欄位 `button1`

這些不會阻止編譯，但屬於可清理項目。

## 風險與實作注意事項

1. `button_close_Click` 不會關閉已啟動的外部程序，只會停止後續自動重啟。
2. `button_run_Click` 與 `myProcess_Exited` 內的 `ProcessStartInfo` 設定不一致。
   `button_run_Click` 使用 `UseShellExecute = false` 並導向標準輸出/錯誤；
   `myProcess_Exited` 重新啟動時改成 `UseShellExecute = true`。
   這代表第一次啟動與自動重啟後的行為可能不同。
3. `button_run_Click` 會 redirect stdout/stderr，但程式沒有讀取輸出內容；若外部程序輸出很多資料，理論上可能造成阻塞風險。
4. `myProcess_Exited` 直接操作 WinForms 控制項，事件回呼執行緒不保證是 UI thread，存在跨執行緒 UI 更新風險。
5. 專案內含 `bin/`、`obj/` 產物；修改時應以 `auto run/` 原始碼為主，不要把建置輸出當成權威來源。

## 建議後續修改方向

如果要繼續維護，優先順序建議如下：

1. 把 `option.ini` 路徑改成以執行檔目錄為準，例如 `AppContext.BaseDirectory`
2. 統一 `ProcessStartInfo` 設定，避免首次啟動與自動重啟邏輯分歧
3. 釐清「關閉」按鈕語意
   如果要真正停止外部程式，需要保留 `Process` 參考並安全地終止它
4. 修正 `Exited` 事件中的 UI thread 存取
5. 移除 designer 裡未使用欄位與 nullable warning

## 接手時的工作準則

- 主要邏輯集中在 `Form1.cs`，先從這裡看行為，不需要先讀完整個 Designer。
- 若要調整 UI 文案或欄位位置，改 `Form1.Designer.cs`；若是改功能，優先改 `Form1.cs`。
- 若要新增更多設定項，需同步處理：
  - `form_auto_run_Load`
  - `form_auto_run_FormClosed`
  - `option.ini`
  - 必要的 UI 控制項
- 不要直接修改 `bin/` 或 `obj/` 下的檔案來實作功能。
