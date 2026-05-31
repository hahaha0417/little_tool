# 程式碼摘要快照

產生時間：2026-05-31 03:51:46
根目錄：`D:\web\web\codex`
摘要檔案數：38

## app/Console/Commands/AI/CacheAiContext.php
- 類型：PHP
- 摘要：用於專案自動化的自訂 Artisan 命令。
- 符號：
  - namespace App\Console\Commands\AI
  - class CacheAiContext
  - extends Command
  - method handle(): int
  - method resolveOutputDir(string $outputDir): string
  - method writeFile(string $outputDir, string $filename, string $contents): void
  - method renderRoutesSummary(): string
  - method renderDatabaseSchemaSummary(): string
  - method renderConfigSummary(): string
  - method summarizeConfigValues(string $configName): array
  - method describeConfig(string $configName): string
  - method renderPackageSummary(): string
  - method mapComposerPackageVersions(array $composerLock): array
  - method mapNpmPackageVersions(array $packageLock): array
  - method describeComposerPackage(string $packageName): string
  - method describeNpmPackage(string $packageName): string
  - method renderTestSummary(): string
  - method renderRecentChangesSummary(): string
  - method renderOwnershipMap(): string
  - method renderPhpSymbolsSummary(): string
  - method extractTestMethods(string $contents): array
  - method matchAll(string $pattern, string $contents): array
  - method extractPhpSymbols(string $contents): array
  - method detectFileKind(string $relativePath): string
  - method ownershipBucket(string $relativePath): string
  - method isRelevantForContext(string $relativePath): bool
  - method formatBytes(int $bytes): string
  - method shouldExclude(string $relativePath): bool
  - method matchFirst(string $pattern, string $contents): ?string
  - method stringifyValue(mixed $value): string
  - method relativePath(SplFileInfo $file): string
  - method displayPath(string $path): string
  - method isAbsolutePath(string $path): bool
  - Route::getRoutes )->getRoutes())
            ->map(function ($route): array {
                $methods = array_values(array_filter(
                    $route->methods()

## app/Console/Commands/AI/CacheCodeSummary.php
- 類型：PHP
- 摘要：用於專案自動化的自訂 Artisan 命令。
- 符號：
  - namespace App\Console\Commands\AI
  - class CacheCodeSummary
  - extends Command
  - method handle(): int
  - method resolveOutputPath(string $outputPath): string
  - method summarizeCodeFiles(): array
  - method relativePath(SplFileInfo $file): string
  - method shouldExclude(string $relativePath): bool
  - method isRelevantCodeFile(string $relativePath): bool
  - method summarizeFile(string $relativePath, string $contents): array
  - method summarizePhpFile(string $relativePath, string $contents): string
  - method extractPhpSymbols(string $contents): array
  - method matchAll(string $pattern, string $contents): array
  - method summarizeJsonFile(string $relativePath, string $contents): string
  - method extractJsonKeys(string $contents): array
  - method renderSummary(array $summaries): string
  - method displayPath(string $path): string
  - method isAbsolutePath(string $path): bool

## app/Console/Commands/AI/CacheProjectStructure.php
- 類型：PHP
- 摘要：用於專案自動化的自訂 Artisan 命令。
- 符號：
  - namespace App\Console\Commands\AI
  - class CacheProjectStructure
  - extends Command
  - method handle(): int
  - method resolveOutputPath(string $outputPath): string
  - method scanDirectory(string $path, string $relativePath = ''): array
  - method shouldExclude(string $relativePath): bool
  - method renderSnapshot(array $entries): string
  - method renderTree(array $entries, string $prefix = ''): array
  - method displayPath(string $path): string
  - method isAbsolutePath(string $path): bool

## app/Http/Controllers/Controller.php
- 類型：PHP
- 摘要：PHP 類別 Controller。
- 符號：
  - namespace App\Http\Controllers
  - class Controller

## app/Models/User.php
- 類型：PHP
- 摘要：定義資料結構與型別轉換規則的 Eloquent 模型。
- 符號：
  - namespace App\Models
  - class User
  - extends Authenticatable
  - method casts(): array

## app/Providers/AppServiceProvider.php
- 類型：PHP
- 摘要：應用程式啟動時使用的服務提供者。
- 符號：
  - namespace App\Providers
  - class AppServiceProvider
  - extends ServiceProvider
  - method register(): void
  - method boot(): void

## artisan
- 類型：程式碼
- 摘要：納入 AI 上下文的專案檔案。

## bootstrap/app.php
- 類型：PHP
- 摘要：應用程式使用的 PHP 原始檔。

## bootstrap/providers.php
- 類型：PHP
- 摘要：應用程式使用的 PHP 原始檔。

## composer.json
- 類型：JSON
- 摘要：Composer 套件定義與 Laravel 相依性。
- 符號：
  - $schema
  - name
  - type
  - description
  - keywords
  - license
  - require
  - require-dev
  - autoload
  - autoload-dev
  - scripts
  - extra
  - config
  - minimum-stability
  - prefer-stable

## config/app.php
- 類型：PHP
- 摘要：應用程式使用的 PHP 原始檔。

## config/auth.php
- 類型：PHP
- 摘要：應用程式使用的 PHP 原始檔。

## config/cache.php
- 類型：PHP
- 摘要：應用程式使用的 PHP 原始檔。

## config/database.php
- 類型：PHP
- 摘要：應用程式使用的 PHP 原始檔。

## config/filesystems.php
- 類型：PHP
- 摘要：應用程式使用的 PHP 原始檔。

## config/logging.php
- 類型：PHP
- 摘要：應用程式使用的 PHP 原始檔。

## config/mail.php
- 類型：PHP
- 摘要：應用程式使用的 PHP 原始檔。

## config/queue.php
- 類型：PHP
- 摘要：應用程式使用的 PHP 原始檔。

## config/services.php
- 類型：PHP
- 摘要：應用程式使用的 PHP 原始檔。

## config/session.php
- 類型：PHP
- 摘要：應用程式使用的 PHP 原始檔。

## database/factories/UserFactory.php
- 類型：PHP
- 摘要：PHP 類別 UserFactory。
- 符號：
  - namespace Database\Factories
  - class UserFactory
  - extends Factory
  - method definition(): array
  - method unverified(): static

## database/migrations/0001_01_01_000000_create_users_table.php
- 類型：PHP
- 摘要：PHP 類別 extends。
- 符號：
  - class extends
  - extends Migration
  - method up(): void
  - method down(): void

## database/migrations/0001_01_01_000001_create_cache_table.php
- 類型：PHP
- 摘要：PHP 類別 extends。
- 符號：
  - class extends
  - extends Migration
  - method up(): void
  - method down(): void

## database/migrations/0001_01_01_000002_create_jobs_table.php
- 類型：PHP
- 摘要：PHP 類別 extends。
- 符號：
  - class extends
  - extends Migration
  - method up(): void
  - method down(): void

## database/seeders/DatabaseSeeder.php
- 類型：PHP
- 摘要：PHP 類別 DatabaseSeeder。
- 符號：
  - namespace Database\Seeders
  - class DatabaseSeeder
  - extends Seeder
  - method run(): void

## package.json
- 類型：JSON
- 摘要：Node 套件定義與前端腳本。
- 符號：
  - $schema
  - private
  - type
  - scripts
  - devDependencies

## phpunit.xml
- 類型：XML
- 摘要：專案設定檔。

## resources/js/app.js
- 類型：程式碼
- 摘要：納入 AI 上下文的專案檔案。

## resources/views/welcome.blade.php
- 類型：PHP
- 摘要：應用程式使用的 PHP 原始檔。
- 符號：
  - Route::has 'login'))
                <nav class="flex items-center justify-end gap-4">
                    @auth
                        <a
                            href="{{ url('/dashboard') }}"
                            class="inline-block px-5 py-1.5 dark:text-[#EDEDEC] border-[#19140035] hover:border-[#1915014a] border text-[#1b1b18] dark:border-[#3E3E3A] dark:hover:border-[#62605b] rounded-sm text-sm leading-normal"
                        >
                            Dashboard
                        </a>
                    @else
                        <a
                            href="{{ route('login') }}"
                            class="inline-block px-5 py-1.5 dark:text-[#EDEDEC] text-[#1b1b18] border border-transparent hover:border-[#19140035] dark:hover:border-[#3E3E3A] rounded-sm text-sm leading-normal"
                        >
                            Log in
                        </a>

                        @if (Route::has('register'))
                            <a
                                href="{{ route('register') }}"
                                class="inline-block px-5 py-1.5 dark:text-[#EDEDEC] border-[#19140035] hover:border-[#1915014a] border text-[#1b1b18] dark:border-[#3E3E3A] dark:hover:border-[#62605b] rounded-sm text-sm leading-normal">
                                Register
                            </a>
                        @endif
                    @endauth
                </nav>
            @endif
        </header>
        <div class="flex items-center justify-center w-full transition-opacity opacity-100 duration-750 lg:grow starting:opacity-0">
            <main class="flex max-w-[335px] w-full flex-col-reverse lg:max-w-4xl lg:flex-row">
                <div class="text-[13px] leading-[20px] flex-1 p-6 pb-6 lg:p-20 lg:pb-10 bg-white dark:bg-[#161615] dark:text-[#EDEDEC] shadow-[inset_0px_0px_0px_1px_rgba(26

## routes/console.php
- 類型：PHP
- 摘要：註冊以 closure 定義的 Artisan 命令。
- 符號：
  - Artisan command 'inspire'

## routes/web.php
- 類型：PHP
- 摘要：定義瀏覽器請求使用的 Web 路由。
- 符號：
  - Route::get '/'

## tests/Feature/AiContextCacheCommandTest.php
- 類型：PHP
- 摘要：應用程式的 PHPUnit 測試覆蓋。
- 符號：
  - namespace Tests\Feature
  - class AiContextCacheCommandTest
  - extends TestCase
  - method test_it_caches_multiple_ai_context_files_for_codex_and_copilot(): void

## tests/Feature/CodeSummaryCacheCommandTest.php
- 類型：PHP
- 摘要：應用程式的 PHPUnit 測試覆蓋。
- 符號：
  - namespace Tests\Feature
  - class CodeSummaryCacheCommandTest
  - extends TestCase
  - method test_it_caches_a_code_summary_for_core_application_files(): void

## tests/Feature/ExampleTest.php
- 類型：PHP
- 摘要：應用程式的 PHPUnit 測試覆蓋。
- 符號：
  - namespace Tests\Feature
  - class ExampleTest
  - extends TestCase
  - method test_the_application_returns_a_successful_response(): void

## tests/Feature/ProjectStructureCacheCommandTest.php
- 類型：PHP
- 摘要：應用程式的 PHPUnit 測試覆蓋。
- 符號：
  - namespace Tests\Feature
  - class ProjectStructureCacheCommandTest
  - extends TestCase
  - method test_it_caches_a_project_structure_snapshot_and_omits_runtime_noise(): void

## tests/TestCase.php
- 類型：PHP
- 摘要：應用程式的 PHPUnit 測試覆蓋。
- 符號：
  - namespace Tests
  - class TestCase
  - extends BaseTestCase

## tests/Unit/ExampleTest.php
- 類型：PHP
- 摘要：應用程式的 PHPUnit 測試覆蓋。
- 符號：
  - namespace Tests\Unit
  - class ExampleTest
  - extends TestCase
  - method test_that_true_is_true(): void

## vite.config.js
- 類型：程式碼
- 摘要：納入 AI 上下文的專案檔案。

