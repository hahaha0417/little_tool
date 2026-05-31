<?php

namespace App\Console\Commands\AI;

use Illuminate\Console\Attributes\Description;
use Illuminate\Console\Attributes\Signature;
use Illuminate\Console\Command;
use Illuminate\Filesystem\Filesystem;
use Illuminate\Support\Carbon;
use SplFileInfo;

#[Signature('app:cache-code-summary {--output= : Output file path}')]
#[Description('為 AI 助手快取精簡程式碼摘要')]
class CacheCodeSummary extends Command
{
    private const DEFAULT_OUTPUT = '.codex/code-summary.md';

    private const EXCLUDED_PATHS = [
        '.codex',
        '.env',
        '.env.backup',
        '.env.example',
        '.env.production',
        '.git',
        '.idea',
        '.nova',
        '.phpunit.cache',
        '.vscode',
        'bootstrap/cache',
        'node_modules',
        'public/build',
        'public/hot',
        'storage/framework',
        'storage/logs',
        'storage/pail',
        'vendor',
    ];

    private const ROOT_FILES = [
        'artisan',
        'composer.json',
        'package.json',
        'phpunit.xml',
        'vite.config.js',
    ];

    private const ALLOWED_EXTENSIONS = [
        'js',
        'json',
        'php',
        'ts',
        'vue',
        'xml',
    ];

    private const ALLOWED_PREFIXES = [
        'app/',
        'bootstrap/',
        'config/',
        'database/',
        'resources/',
        'routes/',
        'tests/',
    ];

    public function __construct(
        private readonly Filesystem $files,
    ) {
        parent::__construct();
    }

    public function handle(): int
    {
        $outputPath = $this->resolveOutputPath((string) $this->option('output'));
        $summaries = $this->summarizeCodeFiles();
        $contents = $this->renderSummary($summaries);


        $this->files->replace($outputPath, $contents);

        $this->components->info(sprintf('程式碼摘要已快取到 %s', $this->displayPath($outputPath)));

        return self::SUCCESS;
    }

    private function resolveOutputPath(string $outputPath): string
    {
        $normalized = trim($outputPath);

        if ($normalized === '') {
            $normalized = self::DEFAULT_OUTPUT;
        }

        if ($this->isAbsolutePath($normalized)) {
            return $normalized;
        }

        return base_path($normalized);
    }

    /**
     * @return array<int, array{path: string, type: string, summary: string, symbols: array<int, string>}>
     */
    private function summarizeCodeFiles(): array
    {
        $summaries = [];

        foreach ($this->files->allFiles(base_path()) as $file) {
            $relativePath = $this->relativePath($file);

            if ($this->shouldExclude($relativePath) || ! $this->isRelevantCodeFile($relativePath)) {
                continue;
            }

            $contents = $this->files->get($file->getPathname());
            $summaries[] = $this->summarizeFile($relativePath, $contents);
        }

        usort(
            $summaries,
            static fn (array $left, array $right): int => strcasecmp($left['path'], $right['path']),
        );

        return $summaries;
    }

    private function relativePath(SplFileInfo $file): string
    {
        $path = str_replace('\\', '/', $file->getPathname());
        $basePath = str_replace('\\', '/', base_path());

        return ltrim(str_replace($basePath, '', $path), '/');
    }

    private function shouldExclude(string $relativePath): bool
    {
        $normalizedPath = str_replace('\\', '/', $relativePath);

        foreach (self::EXCLUDED_PATHS as $excludedPath) {
            if ($normalizedPath === $excludedPath || str_starts_with($normalizedPath, $excludedPath.'/')) {
                return true;
            }
        }

        return false;
    }

    private function isRelevantCodeFile(string $relativePath): bool
    {
        $basename = basename($relativePath);

        if (in_array($basename, self::ROOT_FILES, true)) {
            return true;
        }

        if (str_ends_with($relativePath, '.blade.php')) {
            return true;
        }

        foreach (self::ALLOWED_PREFIXES as $prefix) {
            if (str_starts_with($relativePath, $prefix)) {
                return in_array(pathinfo($relativePath, PATHINFO_EXTENSION), self::ALLOWED_EXTENSIONS, true);
            }
        }

        return false;
    }

    /**
     * @return array{path: string, type: string, summary: string, symbols: array<int, string>}
     */
    private function summarizeFile(string $relativePath, string $contents): array
    {
        if (str_ends_with($relativePath, '.php')) {
            return [
                'path' => $relativePath,
                'type' => 'PHP',
                'summary' => $this->summarizePhpFile($relativePath, $contents),
                'symbols' => $this->extractPhpSymbols($contents),
            ];
        }

        if (str_ends_with($relativePath, '.blade.php')) {
            return [
                'path' => $relativePath,
                'type' => 'Blade',
                'summary' => '前端使用的 Blade 檢視樣板。',
                'symbols' => [],
            ];
        }

        if (str_ends_with($relativePath, '.json')) {
            return [
                'path' => $relativePath,
                'type' => 'JSON',
                'summary' => $this->summarizeJsonFile($relativePath, $contents),
                'symbols' => $this->extractJsonKeys($contents),
            ];
        }

        if (str_ends_with($relativePath, '.xml')) {
            return [
                'path' => $relativePath,
                'type' => 'XML',
                'summary' => '專案設定檔。',
                'symbols' => [],
            ];
        }

        return [
            'path' => $relativePath,
            'type' => '程式碼',
            'summary' => '納入 AI 上下文的專案檔案。',
            'symbols' => [],
        ];
    }

    private function summarizePhpFile(string $relativePath, string $contents): string
    {
        if ($relativePath === 'routes/console.php') {
            return '註冊以 closure 定義的 Artisan 命令。';
        }

        if ($relativePath === 'routes/web.php') {
            return '定義瀏覽器請求使用的 Web 路由。';
        }

        if (str_starts_with($relativePath, 'app/Models/')) {
            return '定義資料結構與型別轉換規則的 Eloquent 模型。';
        }

        if (str_starts_with($relativePath, 'app/Console/Commands/')) {
            return '用於專案自動化的自訂 Artisan 命令。';
        }

        if (str_starts_with($relativePath, 'app/Providers/')) {
            return '應用程式啟動時使用的服務提供者。';
        }

        if (str_starts_with($relativePath, 'tests/')) {
            return '應用程式的 PHPUnit 測試覆蓋。';
        }

        if (preg_match('/class\s+([A-Za-z_][A-Za-z0-9_]*)/m', $contents, $match) === 1) {
            return sprintf('PHP 類別 %s。', $match[1]);
        }

        return '應用程式使用的 PHP 原始檔。';
    }

    /**
     * @return array<int, string>
     */
    private function extractPhpSymbols(string $contents): array
    {
        $symbols = [];

        if (preg_match('/^namespace\s+([^;]+);/m', $contents, $match) === 1) {
            $symbols[] = 'namespace '.$match[1];
        }

        if (preg_match('/\b(class|interface|trait|enum)\s+([A-Za-z_][A-Za-z0-9_]*)/m', $contents, $match) === 1) {
            $symbols[] = sprintf('%s %s', $match[1], $match[2]);
        }

        if (preg_match('/extends\s+([A-Za-z_\\\\][A-Za-z0-9_\\\\]*)/m', $contents, $match) === 1) {
            $symbols[] = 'extends '.$match[1];
        }

        foreach ($this->matchAll('/^\s*(?:final\s+|abstract\s+)?(?:public|protected|private)\s+function\s+([A-Za-z_][A-Za-z0-9_]*)\s*\((.*?)\)\s*(?::\s*([^{\n]+))?/m', $contents) as $method) {
            $signature = $method[1].'('.trim(preg_replace('/\s+/', ' ', $method[2])).')';

            if (isset($method[3]) && trim($method[3]) !== '') {
                $signature .= ': '.trim($method[3]);
            }

            $symbols[] = 'method '.$signature;
        }

        foreach ($this->matchAll('/Artisan::command\(([^,]+),/m', $contents) as $match) {
            $symbols[] = 'Artisan command '.trim($match[1]);
        }

        foreach ($this->matchAll('/Route::([A-Za-z]+)\(([^,]+),/m', $contents) as $match) {
            $symbols[] = sprintf('Route::%s %s', $match[1], trim($match[2]));
        }

        return array_values(array_unique($symbols));
    }

    /**
     * @return array<int, array<int, string>>
     */
    private function matchAll(string $pattern, string $contents): array
    {
        preg_match_all($pattern, $contents, $matches, PREG_SET_ORDER);

        return $matches;
    }

    private function summarizeJsonFile(string $relativePath, string $contents): string
    {
        $decoded = json_decode($contents, true);

        if (! is_array($decoded)) {
            return 'JSON configuration file.';
        }

        if ($relativePath === 'composer.json') {
            return 'Composer 套件定義與 Laravel 相依性。';
        }

        if ($relativePath === 'package.json') {
            return 'Node 套件定義與前端腳本。';
        }

        return 'JSON 設定檔。';
    }

    /**
     * @return array<int, string>
     */
    private function extractJsonKeys(string $contents): array
    {
        $decoded = json_decode($contents, true);

        if (! is_array($decoded)) {
            return [];
        }

        return array_map(
            static fn (string|int $key): string => (string) $key,
            array_keys($decoded),
        );
    }

    /**
     * @param array<int, array{path: string, type: string, summary: string, symbols: array<int, string>}> $summaries
     */
    private function renderSummary(array $summaries): string
    {
        $lines = [
            '# 程式碼摘要快照',
            '',
            '產生時間：'.Carbon::now()->toDateTimeString(),
            '根目錄：`'.base_path().'`',
            '摘要檔案數：'.count($summaries),
            '',
        ];

        foreach ($summaries as $summary) {
            $lines[] = '## '.$summary['path'];
            $lines[] = '- 類型：'.$summary['type'];
            $lines[] = '- 摘要：'.$summary['summary'];

            if ($summary['symbols'] !== []) {
                $lines[] = '- 符號：';

                foreach ($summary['symbols'] as $symbol) {
                    $lines[] = '  - '.$symbol;
                }
            }

            $lines[] = '';
        }

        return implode(PHP_EOL, $lines).PHP_EOL;
    }

    private function displayPath(string $path): string
    {
        $basePath = str_replace('\\', '/', base_path());
        $normalizedPath = str_replace('\\', '/', $path);

        if (str_starts_with($normalizedPath, $basePath.'/')) {
            return substr($normalizedPath, strlen($basePath) + 1);
        }

        return $normalizedPath;
    }

    private function isAbsolutePath(string $path): bool
    {
        return preg_match('/^(?:[A-Za-z]:[\\\\\\/]|[\\\\\\/]{2}|\/)/', $path) === 1;
    }
}
