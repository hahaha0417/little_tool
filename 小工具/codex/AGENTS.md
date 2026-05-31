# Laravel Boost 指南

<laravel-boost-guidelines>
=== 基礎規則 ===

# Laravel Boost 指南

這份指南由 Laravel 維護者為此應用程式整理，請務必遵守，以維持最佳的 Laravel 開發體驗。

## 基礎背景

這是一個 Laravel 應用程式，主要套件與版本如下。你應該熟悉並遵守這些版本。

- php - 8.3
- laravel/framework (LARAVEL) - v13
- laravel/prompts (PROMPTS) - v0
- laravel/boost (BOOST) - v2
- laravel/mcp (MCP) - v0
- laravel/pail (PAIL) - v1
- laravel/pint (PINT) - v1
- phpunit/phpunit (PHPUNIT) - v12
- tailwindcss (TAILWINDCSS) - v4

## 技能啟用

此專案在 `**/skills/**` 下提供領域專用技能。只要工作內容屬於 해당領域，就必須啟用對應技能，不要等到卡住才用。

## 約定

- 變更或建立檔案時，先參考相鄰檔案的結構、做法與命名，並遵守現有程式風格。
- 變數與方法名稱要具描述性，例如 `isRegisteredForDiscounts`，不要寫成 `discount()`。
- 先確認是否已經有可重用的元件，再考慮建立新元件。

## 驗證腳本

- 如果測試已經足以驗證功能，就不要再額外建立驗證腳本或使用 tinker。
- 單元測試與功能測試比臨時腳本更重要。

## 應用程式架構

- 維持既有目錄結構，不要未經允許就建立新的基礎資料夾。
- 未經允許不要更動專案相依套件。

## 前端打包

- 如果前端修改沒有反映到 UI，可能需要執行 `npm run build`、`npm run dev` 或 `composer run dev`。必要時請提醒使用者。

## 文件檔案

- 除非使用者明確要求，否則不要建立文件類檔案。

## 回覆

- 說明要簡潔，聚焦重點，不要把顯而易見的內容重講一遍。

=== Boost 規則 ===

# Laravel Boost

## 工具

- Laravel Boost 提供這個應用程式專用的 MCP 工具，優先使用 Boost 工具，不要先選手動方式。
- 讀寫資料庫時，請用 `database-query` 做唯讀查詢，不要在 tinker 裡直接寫原始 SQL。
- 寫 migration 或 model 前，先用 `database-schema` 檢查表結構。
- 分享專案 URL 前，一律先用 `get-absolute-url` 取得正確的 scheme、domain 與 port。
- 讀 browser logs、錯誤與 exception 時，使用 `browser-logs`，而且只看近期紀錄。

## 專案上下文快取

- 在這個 repository 開始任何工作前，先讀 `.codex/code-summary.md`、`.codex/project-structure.md`，以及 `storage/app/ai-context/` 下由 `app:cache-ai-context` 產生的快取檔。
- 如果任一檔案不存在或明顯過期，先重新產生快取，再繼續後續工作。

## 搜尋文件（重要）

- 在修改程式碼前，一定要先用 `search-docs`。不要跳過這一步。
- 當你知道相關套件時，請傳入 `packages` 限定搜尋範圍。
- 使用多個主題式查詢，例如：`['rate limiting', 'routing rate limiting', 'routing']`。通常最相關的結果會排在前面。
- 不要在查詢字串中加入套件名稱，因為套件資訊已經由工具提供。請用 `test resource table`，不要寫 `filament 4 test resource table`。

### 搜尋語法

1. 使用單字可套用詞幹比對的 AND 邏輯，例如 `rate limit` 會同時比對 `rate` 與 `limit`。
2. 使用雙引號包住片語，可要求精確相鄰比對，例如 `"infinite scroll"`。
3. 混合使用單字與片語，例如 `middleware "rate limit"`。
4. 使用多個查詢模擬 OR 邏輯，例如 `queries=["authentication", "middleware"]`。

## Artisan

- 直接執行 Artisan 指令，例如 `php artisan route:list`。可先用 `php artisan list` 查看可用指令，再用 `php artisan [command] --help` 查看參數。
- 使用 `php artisan route:list` 檢查路由，可搭配 `--method=GET`、`--name=users`、`--path=api`、`--except-vendor`、`--only-vendor` 篩選。
- 讀取設定值時，請使用 dot notation，例如 `php artisan config:show app.name`、`php artisan config:show database.default`。也可以直接讀 `config/` 目錄中的設定檔。

## Tinker

- 在應用程式脈絡中執行 PHP 時，優先用 tinker 做除錯與驗證。
- 不要為了測試而建立 model，應優先使用 factories。
- 若已有合適的 Artisan 命令，優先使用它，不要自己額外寫 tinker 腳本。
- 指令字串請使用單引號避免 shell 展開：`php artisan tinker --execute 'Your::code();'`
  - PHP 內部字串再用雙引號：`php artisan tinker --execute 'User::where("active", true)->count();'`

=== PHP 規則 ===

# PHP

- 控制結構一律加大括號，即使只有一行也要寫。
- 使用 PHP 8 建構子屬性提升，例如 `public function __construct(public GitHub $github) { }`。
- 不要留下空的無參數 `__construct()`，除非它是 private。
- 所有方法參數與回傳值都要有明確型別宣告。
- Enum key 使用 TitleCase，例如 `FavoritePerson`、`BestLake`、`Monthly`。
- 偏好使用 PHPDoc，而不是 inline comment。只有在非常複雜的邏輯才加 inline comment。
- PHPDoc 請使用 array shape 型別定義。

=== 部署規則 ===

# Deployment

- Laravel 可以部署到 [Laravel Cloud](https://cloud.laravel.com/)，這是最快的生產環境部署與擴充方式。

=== Laravel 核心規則 ===

# 用 Laravel 的方式做事

- 建立新檔案時，優先使用 `php artisan make:` 指令，例如 migration、controller、model 等。
- 可以先用 `php artisan list` 查看可用指令，再用 `php artisan [command] --help` 確認參數。
- 建立一般 PHP class 時，請用 `php artisan make:class`。
- 所有 Artisan 指令都請加上 `--no-interaction`，並搭配正確選項，確保可自動執行。

### Model 建立

- 新增 model 時，也要一起建立有用的 factory 與 seeder。
- 若有其他相關產物需要建立，先看 `php artisan make:model --help` 是否支援。

## API 與 Eloquent Resources

- API 預設使用 Eloquent API Resources 與 API versioning；若現有 API 路由不是這種做法，則遵循現有慣例。

## URL 產生

- 產生頁面連結時，優先使用 named route 與 `route()`。

## 測試

- 建立測試用 model 時，請使用 factory。先確認 factory 是否已有可用的 custom state，再考慮手動設定。
- Faker 請依現有慣例使用 `$this->faker` 或 `fake()`。
- 建立測試時，請使用 `php artisan make:test [options] {name}`，若是 unit test 則加上 `--unit`。
- 大多數測試應該是 feature test。

## Vite 錯誤

- 如果出現 `Illuminate\Foundation\ViteException: Unable to locate file in Vite manifest`，可以執行 `npm run build`，或請使用者執行 `npm run dev` / `composer run dev`。

=== Pint 規則 ===

# Laravel Pint 格式化器

- 只要有修改 PHP 檔案，結束前一定要執行 `vendor/bin/pint --dirty --format agent`。
- 不要執行 `vendor/bin/pint --test --format agent`，請直接執行 `vendor/bin/pint --format agent` 來修正格式。

=== PHPUnit 規則 ===

# PHPUnit

- 本專案使用 PHPUnit。所有測試都必須是 PHPUnit class，請用 `php artisan make:test --phpunit {name}` 建立。
- 如果看到 Pest 測試，請改成 PHPUnit。
- 每次更新測試後，都要執行對應的單一測試。
- 與本次變更相關的測試都通過後，請詢問使用者是否也要跑整個 test suite。
- 測試應涵蓋 happy path、failure path 與 edge case。
- 未經允許，不要刪除 `tests` 目錄中的任何測試檔或測試資料夾。

## 執行測試

- 優先用最少的測試數量與適當的 filter 來驗證變更。
- 執行全部測試：`php artisan test --compact`
- 執行單一檔案全部測試：`php artisan test --compact tests/Feature/ExampleTest.php`
- 針對特定測試名稱過濾：`php artisan test --compact --filter=testName`

</laravel-boost-guidelines>
