# AGENTS.md

## 專案概要

此專案是一個 Windows Forms 桌面工具，用來管理多組外部程式的啟動、停止、批次啟動、批次停止，以及可選的自動重啟。

- 解決方案：`D:\vs\c#\auto run multiple_advance\auto run multiple.sln`
- 主程式：`D:\vs\c#\auto run multiple_advance\auto run multiple`
- 共用類庫：`D:\vs\c#\auto run multiple_advance\hahahalib`

## 技術棧

- .NET WinForms
- 主程式 Target Framework：`net9.0-windows7.0`
- 類庫 Target Framework：`net9.0-windows`
- JSON 序列化：`System.Text.Json`
- 類庫相依：
  - `Microsoft.Extensions.Configuration.*`
  - `Microsoft.Extensions.Logging.*`
  - `Newtonsoft.Json`
  - `NLog`
  - `SkiaSharp`
  - `Emgu.CV`

注意：目前主程式實際有用到的核心功能仍以 WinForms、`System.Diagnostics.Process` 與本地 JSON 設定檔為主，圖像/執行緒工具庫多半尚未整合進主流程。

## 啟動流程

入口在 `D:\vs\c#\auto run multiple_advance\auto run multiple\Program.cs`。

啟動順序如下：

1. `ApplicationConfiguration.Initialize()`
2. `hahaha.Initial_Environment()`
3. 建立 `form_auto_run_multiple`
4. 將主表單註冊到靜態全域欄位
5. `hahaha.Initial()`
6. `hahaha.Initial_UI()`
7. `Application.Run(form_main_)`

目前真正有做事的是 `Initial_Environment()`：

- 建立 `hahaha_setting_box`
- 讀取全部設定
- 若載入失敗則立即產生預設設定並存回磁碟
- 將設定物件掛到 `ha.Setting`

`Initial()` 與 `Initial_UI()` 目前是空實作。

## 核心資料模型

設定集中在 `D:\vs\c#\auto run multiple_advance\auto run multiple\box\hahaha_setting_box.cs`。

它封裝三份設定：

- `System`：系統 UI 設定
- `Global`：全域設定，目前幾乎為空
- `Setting`：實際的 class/item 程式清單

### system.json

定義於 `D:\vs\c#\auto run multiple_advance\auto run multiple\setting\hahaha_setting_system.cs`

- 路徑固定在 `setting/system/system.json`
- 目前欄位：
  - `Count_Line`
  - `Time_Display`

### global.json

定義於 `D:\vs\c#\auto run multiple_advance\auto run multiple\setting\hahaha_setting_global.cs`

- 路徑固定在 `setting/global/global.json`
- 目前沒有實際業務欄位

### setting.json

定義於 `D:\vs\c#\auto run multiple_advance\auto run multiple\setting\hahaha_setting_setting.cs`

- 路徑固定在 `setting/setting/setting.json`
- 結構：
  - `hahaha_setting_setting`
  - `List<hahaha_setting_system_class>`
  - `List<hahaha_setting_system_item>`

每個 item 的持久化欄位：

- `Auto_Reload`
- `Use_Shell_Excute`
- `Create_No_Window`
- `Name`
- `Command`
- `Parameter`

不會被序列化的執行期欄位：

- `Status`
- `Process`
- `Running`

這代表重開程式後只會保留配置，不會保留執行中的程序狀態。

## 主 UI 與功能對應

主表單在：

- `D:\vs\c#\auto run multiple_advance\auto run multiple\Form1.cs`
- `D:\vs\c#\auto run multiple_advance\auto run multiple\Form1.Designer.cs`

UI 大致分成三塊：

- 左側 `box_class`：class 清單
- 中間 `box_item`：該 class 底下的 item 清單
- 右側 tab：
  - `設定`
  - `輸出`
  - `系統設定`

### 單一 item 操作

- 執行：`button_run_Click`
- 關閉旗標：`button_close_Click`
- 強制 kill：`button_kill_process_Click`

### 批次操作

- 選取 item / class / 全部執行：`button_run_select_Click`
- 選取 item / class / 全部關閉：`button_close_select_Click`
- 選取 item / class / 全部 kill：`button_kill_process_select_Click`

### 編輯設定

- 新增 / 更名 / 刪除 / 上下移動 / 複製 class
- 新增 / 更名 / 刪除 / 上下移動 / 複製 item
- 直接編輯 `Command` / `Parameter`
- 勾選：
  - 自動重載
  - Shell 執行
  - 不建立視窗

### 設定檔操作

- 載入全部：`button_load_all_Click`
- 儲存全部：`button_save_all_Click`
- 重設全部：`button_reset_all_Click`

## 子程序啟動行為

核心依賴 `System.Diagnostics.Process`。

### `Use_Shell_Excute = true`

- `UseShellExecute = true`
- 不會 redirect stdout/stderr
- 適合交給系統預設程式或 shell 開啟

### `Use_Shell_Excute = false`

- `UseShellExecute = false`
- `RedirectStandardOutput = true`
- `RedirectStandardError = true`
- 可作為 console 程式啟動

### 自動重啟

`myProcess_Exited()` 會在程序結束後檢查：

- 若 `Running == true`
- 且 `Auto_Reload == true`

則重新建立 `Process` 並再次啟動。

## 已確認的現況與限制

### 1. 「關閉」多數只是關掉旗標，不一定真的終止程序

`button_close_Click` 與批次 close 系列主要是把 `Running` 設成 `false`，並沒有呼叫 `CloseMainWindow()`、`Kill()` 或其他終止流程。

實際上：

- 程式可能仍在背景執行
- 只是停止後續自動重啟
- 真正強制終止要另外按 kill

### 2. 批次執行忽略 item 自身設定

`button_run_select_Click` 內部建立 `ProcessStartInfo` 時寫死：

- `UseShellExecute = false`
- `RedirectStandardOutput = true`
- `RedirectStandardError = true`
- `CreateNoWindow = false`

這會覆蓋 item 原本的：

- `Use_Shell_Excute`
- `Create_No_Window`

所以單筆執行與批次執行的行為不一致。

### 3. 有輸出 UI，但沒有真正接線

雖然 UI 有：

- `box_display`
- `Count_Line`
- `Time_Display`

但目前程式碼沒有看到：

- `OutputDataReceived`
- `ErrorDataReceived`
- 背景輪詢 stdout/stderr
- 任何把輸出寫進 `box_display` 的邏輯

因此「輸出」tab 與相關系統設定目前幾乎是未完成功能。

### 4. 結束程式時未做統一清理

`form_auto_run_FormClosed` 目前為空。

風險：

- 子程序可能留在背景
- 設定可能未自動儲存
- `hahaha.Close()` 未被呼叫

### 5. 大量依賴靜態全域狀態

主程式使用多個靜態欄位：

- `hahaha.Form_Main_`
- `hahaha.Setting_Box_`
- `ha.Form_Main`
- `ha.Setting`

優點是簡單直接；缺點是測試性差、耦合高，後續要抽背景服務或重構 UI 邏輯時成本會上升。

### 6. 命名與拼字沿用既有風格

專案中存在明顯歷史命名，例如：

- `Use_Shell_Excute`
- `hahaha_*`

如果後續修改，預設應先保持既有命名風格，避免在無必要時混入大規模 rename。

## `hahahalib` 的角色

`hahahalib` 是作者自己的輕量共用庫。README 明確寫到：

- 偏工作內部使用
- 偏 lite
- 沒有明確維護規劃
- 刻意降低 library 複雜度

目前主專案實際用到的類庫能力主要是：

- `hahaha_json`
- 部分全域類別與命名空間承接

其他模組如：

- 表單基底
- thread command
- timer
- gps
- skia ui control

在目前主專案中沒有看到明確整合進主要流程。

## 後續修改建議

若後續代理要在此專案工作，建議優先順序如下：

1. 先確認要改的是「配置編輯器」還是「程序執行器」
2. 若碰到程序啟停問題，先讀 `Form1.cs` 的 run/close/kill/exit 流程
3. 若碰到資料遺失問題，先讀 `hahaha_setting_box.cs` 與 `hahaha_json.cs`
4. 若要補輸出視窗，優先補：
   - `OutputDataReceived`
   - `ErrorDataReceived`
   - UI thread marshal
   - `Count_Line` 截斷策略
5. 若要補正批次行為，先讓批次執行尊重 item 自身設定
6. 若要補安全退出，先在 `FormClosed` 統一處理：
   - 停止自動重啟
   - 視需要終止子程序
   - 儲存設定

## 建議閱讀順序

1. `D:\vs\c#\auto run multiple_advance\auto run multiple\Program.cs`
2. `D:\vs\c#\auto run multiple_advance\auto run multiple\define\hahaha_define.cs`
3. `D:\vs\c#\auto run multiple_advance\auto run multiple\Form1.cs`
4. `D:\vs\c#\auto run multiple_advance\auto run multiple\box\hahaha_setting_box.cs`
5. `D:\vs\c#\auto run multiple_advance\auto run multiple\setting\hahaha_setting_setting.cs`
6. `D:\vs\c#\auto run multiple_advance\hahahalib\hahahalib\json\hahaha_json.cs`

## 本次分析範圍

本文件根據目前工作區內可讀到的原始碼整理，重點放在：

- 專案結構
- 啟動流程
- 設定落地方式
- 主功能與 UI 對應
- 已存在但未完成或行為不一致的部分

未包含：

- 實機執行驗證
- UI 視覺調整
- 第三方套件深入用途追查
