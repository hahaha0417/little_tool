<?php

namespace App\Console\Commands\AI;

use Illuminate\Console\Attributes\Description;
use Illuminate\Console\Attributes\Signature;
use Illuminate\Console\Command;
use Illuminate\Filesystem\Filesystem;
use Illuminate\Support\Carbon;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Route;
use Illuminate\Support\Facades\Schema;
use Symfony\Component\Finder\SplFileInfo;

#[Signature('app:cache-ai-context {--output-dir=storage/app/ai-context : Directory used to store AI context cache files}')]
#[Description('為 AI 助手快取多份專案上下文檔案')]
class CacheAiContext extends Command
{
    private const DEFAULT_OUTPUT_DIR = 'storage/app/ai-context';

    private const CONFIG_FILES = [
        'app',
        'auth',
        'cache',
        'database',
        'filesystems',
        'logging',
        'mail',
        'queue',
        'services',
        'session',
    ];

    private const COMPOSER_PACKAGES = [
        'laravel/framework',
        'laravel/boost',
        'laravel/pail',
        'laravel/pint',
        'laravel/tinker',
        'phpunit/phpunit',
    ];

    private const NPM_PACKAGES = [
        '@tailwindcss/vite',
        'concurrently',
        'laravel-vite-plugin',
        'tailwindcss',
        'vite',
    ];

    public function __construct(
        private readonly Filesystem $files,
    ) {
        parent::__construct();
    }

    public function handle(): int
    {
        $outputDir = $this->resolveOutputDir((string) $this->option('output-dir'));
        $this->files->ensureDirectoryExists($outputDir);

        $this->writeFile($outputDir, 'routes.md', $this->renderRoutesSummary());
        $this->writeFile($outputDir, 'database-schema.md', $this->renderDatabaseSchemaSummary());
        $this->writeFile($outputDir, 'config.md', $this->renderConfigSummary());
        $this->writeFile($outputDir, 'packages.md', $this->renderPackageSummary());
        $this->writeFile($outputDir, 'tests.md', $this->renderTestSummary());
        $this->writeFile($outputDir, 'recent-changes.md', $this->renderRecentChangesSummary());
        $this->writeFile($outputDir, 'ownership-map.md', $this->renderOwnershipMap());
        $this->writeFile($outputDir, 'php-symbols.md', $this->renderPhpSymbolsSummary());

        $this->components->info(sprintf('AI 上下文快取已輸出到 %s', $this->displayPath($outputDir)));

        return self::SUCCESS;
    }

    private function resolveOutputDir(string $outputDir): string
    {
        $normalized = trim($outputDir);

        if ($normalized === '') {
            $normalized = self::DEFAULT_OUTPUT_DIR;
        }

        if ($this->isAbsolutePath($normalized)) {
            return $normalized;
        }

        return base_path($normalized);
    }

    private function writeFile(string $outputDir, string $filename, string $contents): void
    {
        $fullPath = $outputDir.DIRECTORY_SEPARATOR.$filename;
        if ($this->files->exists($fullPath)) {
            $current = $this->files->get($fullPath);
            if ($current === $contents) {
                // 檔案內容未變動，略過處理
                return;
            }
        }
        $this->files->replace($fullPath, $contents);
    }

    private function renderRoutesSummary(): string
    {
        $routes = collect(Route::getRoutes()->getRoutes())
            ->map(function ($route): array {
                $methods = array_values(array_filter(
                    $route->methods(),
                    static fn (string $method): bool => $method !== 'HEAD',
                ));

                return [
                    'methods' => $methods,
                    'uri' => $route->uri(),
                    'name' => $route->getName() ?? '',
                    'action' => $route->getActionName(),
                    'middleware' => array_values(array_unique($route->gatherMiddleware())),
                ];
            })
            ->sortBy('uri')
            ->values();

        $lines = [
            '# 路由快取',
            '',
            '產生時間：'.Carbon::now()->toDateTimeString(),
            '總路由數：'.$routes->count(),
            '',
        ];

        foreach ($routes as $route) {
            $lines[] = '## '.($route['name'] !== '' ? $route['name'] : $route['uri']);
            $lines[] = '- 方法：'.implode('|', $route['methods']);
            $lines[] = '- URI：'.$route['uri'];
            $lines[] = '- 動作：'.$route['action'];

            if ($route['middleware'] !== []) {
                $lines[] = '- Middleware：'.implode(', ', $route['middleware']);
            }

            $lines[] = '';
        }

        return implode(PHP_EOL, $lines).PHP_EOL;
    }

    private function renderDatabaseSchemaSummary(): string
    {
        $database = DB::getDatabaseName();
        $tables = Schema::getTables();

        $lines = [
            '# 資料庫結構快取',
            '',
            '產生時間：'.Carbon::now()->toDateTimeString(),
            '資料庫：'.$database,
            '資料表數：'.count($tables),
            '',
        ];

        foreach ($tables as $table) {
            $tableName = $table['name'] ?? $table['schema_qualified_name'];
            $lines[] = '## '.$tableName;

            foreach (Schema::getColumns($tableName) as $column) {
                $details = [$column['type_name']];

                if (($column['nullable'] ?? false) === false) {
                    $details[] = 'not null';
                }

                if (($column['default'] ?? null) !== null) {
                    $details[] = '預設：'.$this->stringifyValue($column['default']);
                }

                if (! empty($column['auto_increment'])) {
                    $details[] = 'auto_increment';
                }

                $lines[] = sprintf('- %s: %s', $column['name'], implode(' / ', $details));
            }

            $indexes = Schema::getIndexes($tableName);
            if ($indexes !== []) {
                $lines[] = '- 索引：';

                foreach ($indexes as $index) {
                    $lines[] = sprintf('  - %s (%s)', $index['name'], implode(', ', $index['columns']));
                }
            }

            $foreignKeys = Schema::getForeignKeys($tableName);
            if ($foreignKeys !== []) {
                $lines[] = '- 外鍵：';

                foreach ($foreignKeys as $foreignKey) {
                    $lines[] = sprintf(
                        '  - %s -> %s (%s)',
                        implode(', ', $foreignKey['columns']),
                        $foreignKey['foreign_table'],
                        implode(', ', $foreignKey['foreign_columns']),
                    );
                }
            }

            $lines[] = '';
        }

        return implode(PHP_EOL, $lines).PHP_EOL;
    }

    private function renderConfigSummary(): string
    {
        $lines = [
            '# 設定快取',
            '',
            '產生時間：'.Carbon::now()->toDateTimeString(),
            '',
        ];

        foreach (self::CONFIG_FILES as $configName) {
            $lines[] = '## config/'.$configName.'.php';
            $lines[] = '- 說明：'.$this->describeConfig($configName);

            foreach ($this->summarizeConfigValues($configName) as $summaryLine) {
                $lines[] = '- '.$summaryLine;
            }

            $lines[] = '';
        }

        return implode(PHP_EOL, $lines).PHP_EOL;
    }

    /**
     * @return array<int, string>
     */
    private function summarizeConfigValues(string $configName): array
    {
        return match ($configName) {
            'app' => [
                'name：'.config('app.name'),
                'env：'.config('app.env'),
                'debug：'.(config('app.debug') ? 'true' : 'false'),
                'timezone：'.config('app.timezone'),
                'locale：'.config('app.locale'),
            ],
            'auth' => [
                'guard：'.(string) config('auth.defaults.guard'),
                'passwords：'.(string) config('auth.defaults.passwords'),
            ],
            'cache' => [
                'default：'.(string) config('cache.default'),
                'prefix：'.(string) config('cache.prefix'),
            ],
            'database' => [
                'default：'.(string) config('database.default'),
                'connections：'.implode(', ', array_keys((array) config('database.connections'))),
            ],
            'filesystems' => [
                'default：'.(string) config('filesystems.default'),
                'disks：'.implode(', ', array_keys((array) config('filesystems.disks'))),
            ],
            'logging' => [
                'default：'.(string) config('logging.default'),
                'channels：'.implode(', ', array_keys((array) config('logging.channels'))),
            ],
            'mail' => [
                'default：'.(string) config('mail.default'),
                'from.address：'.(string) config('mail.from.address'),
            ],
            'queue' => [
                'default：'.(string) config('queue.default'),
            ],
            'services' => [
                'keys：'.implode(', ', array_keys((array) config('services'))),
            ],
            'session' => [
                'driver：'.(string) config('session.driver'),
                'lifetime：'.(string) config('session.lifetime'),
            ],
            default => [],
        };
    }

    private function describeConfig(string $configName): string
    {
        return match ($configName) {
            'app' => '應用程式基本資訊與環境設定。',
            'auth' => '驗證守衛與密碼重設設定。',
            'cache' => '快取驅動與前綴設定。',
            'database' => '資料庫連線與連線名稱。',
            'filesystems' => '檔案系統磁碟與預設磁碟。',
            'logging' => '日誌通道與預設處理器。',
            'mail' => '郵件傳送器與寄件人設定。',
            'queue' => '佇列驅動與預設佇列。',
            'services' => '外部服務金鑰與設定。',
            'session' => 'Session 驅動與逾時設定。',
            default => '設定檔摘要。',
        };
    }

    private function renderPackageSummary(): string
    {
        $composer = json_decode((string) file_get_contents(base_path('composer.json')), true, 512, JSON_THROW_ON_ERROR);
        $composerLock = json_decode((string) file_get_contents(base_path('composer.lock')), true, 512, JSON_THROW_ON_ERROR);
        $packageJson = json_decode((string) file_get_contents(base_path('package.json')), true, 512, JSON_THROW_ON_ERROR);
        $packageLock = json_decode((string) file_get_contents(base_path('package-lock.json')), true, 512, JSON_THROW_ON_ERROR);

        $composerVersions = $this->mapComposerPackageVersions($composerLock);
        $npmVersions = $this->mapNpmPackageVersions($packageLock);

        $lines = [
            '# 套件快取',
            '',
            '產生時間：'.Carbon::now()->toDateTimeString(),
            '',
            '## composer.json',
            '- description：'.($composer['description'] ?? ''),
            '- require：'.implode(', ', array_keys($composer['require'] ?? [])),
            '- require-dev：'.implode(', ', array_keys($composer['require-dev'] ?? [])),
            '',
        ];

        foreach (self::COMPOSER_PACKAGES as $packageName) {
            if (! array_key_exists($packageName, $composerVersions)) {
                continue;
            }

            $lines[] = sprintf('## %s', $packageName);
            $lines[] = '- 版本：'.$composerVersions[$packageName];
            $lines[] = '- 說明：'.$this->describeComposerPackage($packageName);
            $lines[] = '';
        }

        $lines[] = '## package.json';
        $lines[] = '- name：'.($packageJson['name'] ?? '前端套件定義');
        $lines[] = '- description：'.($packageJson['description'] ?? '');
        $lines[] = '- devDependencies：'.implode(', ', array_keys($packageJson['devDependencies'] ?? []));
        $lines[] = '';

        foreach (self::NPM_PACKAGES as $packageName) {
            if (! array_key_exists($packageName, $npmVersions)) {
                continue;
            }

            $lines[] = sprintf('## %s', $packageName);
            $lines[] = '- 版本：'.$npmVersions[$packageName];
            $lines[] = '- 說明：'.$this->describeNpmPackage($packageName);
            $lines[] = '';
        }

        return implode(PHP_EOL, $lines).PHP_EOL;
    }

    /**
     * @param array<string, mixed> $composerLock
     * @return array<string, string>
     */
    private function mapComposerPackageVersions(array $composerLock): array
    {
        $versions = [];

        foreach (array_merge($composerLock['packages'] ?? [], $composerLock['packages-dev'] ?? []) as $package) {
            if (! isset($package['name'], $package['version'])) {
                continue;
            }

            $versions[$package['name']] = $package['version'];
        }

        return $versions;
    }

    /**
     * @param array<string, mixed> $packageLock
     * @return array<string, string>
     */
    private function mapNpmPackageVersions(array $packageLock): array
    {
        $versions = [];

        foreach (($packageLock['packages'] ?? []) as $path => $package) {
            if (! is_string($path) || ! isset($package['version'])) {
                continue;
            }

            $packageName = basename($path);

            if ($packageName === '') {
                continue;
            }

            $versions[$packageName] = $package['version'];
        }

        return $versions;
    }

    private function describeComposerPackage(string $packageName): string
    {
        return match ($packageName) {
            'laravel/framework' => 'Laravel 核心框架。',
            'laravel/boost' => 'Laravel Boost 工具與規則。',
            'laravel/pail' => '即時日誌尾隨工具。',
            'laravel/pint' => '程式碼格式化工具。',
            'laravel/tinker' => '互動式應用程式執行環境。',
            'phpunit/phpunit' => 'PHP 單元與功能測試框架。',
            default => '已安裝套件。',
        };
    }

    private function describeNpmPackage(string $packageName): string
    {
        return match ($packageName) {
            '@tailwindcss/vite' => 'Tailwind CSS 與 Vite 整合。',
            'concurrently' => '同時執行多個開發命令。',
            'laravel-vite-plugin' => 'Laravel 的 Vite 整合外掛。',
            'tailwindcss' => '前端樣式工具。',
            'vite' => '前端建置工具。',
            default => '已安裝套件。',
        };
    }

    private function renderTestSummary(): string
    {
        $testFiles = collect($this->files->allFiles(base_path('tests')))
            ->filter(static fn (SplFileInfo $file): bool => $file->getExtension() === 'php')
            ->map(function (SplFileInfo $file): array {
                $relativePath = $this->relativePath($file);
                $contents = $this->files->get($file->getPathname());

                return [
                    'path' => $relativePath,
                    'class' => $this->matchFirst('/class\s+([A-Za-z_][A-Za-z0-9_]*)/m', $contents) ?? pathinfo($relativePath, PATHINFO_FILENAME),
                    'methods' => $this->extractTestMethods($contents),
                ];
            })
            ->sortBy('path')
            ->values();

        $lines = [
            '# 測試映射快取',
            '',
            '產生時間：'.Carbon::now()->toDateTimeString(),
            '測試檔案數：'.$testFiles->count(),
            '',
        ];

        foreach ($testFiles as $testFile) {
            $lines[] = '## '.$testFile['path'];
            $lines[] = '- 類別：'.$testFile['class'];
            $lines[] = '- 測試方法：'.($testFile['methods'] !== [] ? implode(', ', $testFile['methods']) : '無');
            $lines[] = '';
        }

        return implode(PHP_EOL, $lines).PHP_EOL;
    }

    private function renderRecentChangesSummary(): string
    {
        $recentFiles = collect($this->files->allFiles(base_path()))
            ->filter(function (SplFileInfo $file): bool {
                $relativePath = $this->relativePath($file);

                return $this->isRelevantForContext($relativePath);
            })
            ->sortByDesc(fn (SplFileInfo $file): int => $file->getMTime())
            ->take(20)
            ->values();

        $lines = [
            '# 最近變更快取',
            '',
            '產生時間：'.Carbon::now()->toDateTimeString(),
            '說明：依檔案修改時間推定最近變更，非 git 歷史。',
            '',
        ];

        foreach ($recentFiles as $file) {
            $relativePath = $this->relativePath($file);
            $modifiedAt = Carbon::createFromTimestamp($file->getMTime())->toDateTimeString();

            $lines[] = '## '.$relativePath;
            $lines[] = '- 最近修改：'.$modifiedAt;
            $lines[] = '- 大小：'.$this->formatBytes($file->getSize());
            $lines[] = '- 類別：'.$this->detectFileKind($relativePath);
            $lines[] = '';
        }

        return implode(PHP_EOL, $lines).PHP_EOL;
    }

    private function renderOwnershipMap(): string
    {
        $files = collect($this->files->allFiles(base_path()))
            ->filter(fn (SplFileInfo $file): bool => $this->isRelevantForContext($this->relativePath($file)))
            ->values();

        $groups = $files->groupBy(fn (SplFileInfo $file): string => $this->ownershipBucket($this->relativePath($file)));

        $lines = [
            '# 責任地圖快取',
            '',
            '產生時間：'.Carbon::now()->toDateTimeString(),
            '說明：此檔為依路徑與命名空間推定的責任分區，並非 git 作者統計。',
            '',
        ];

        foreach ($groups->sortKeys() as $bucket => $groupFiles) {
            $lines[] = '## '.$bucket;
            $lines[] = '- 檔案數：'.$groupFiles->count();
            $lines[] = '- 主要檔案：';

            foreach ($groupFiles->take(8) as $file) {
                $lines[] = '  - '.$this->relativePath($file);
            }

            $lines[] = '';
        }

        return implode(PHP_EOL, $lines).PHP_EOL;
    }

    private function renderPhpSymbolsSummary(): string
    {
        $phpFiles = collect($this->files->allFiles(base_path()))
            ->filter(function (SplFileInfo $file): bool {
                $relativePath = $this->relativePath($file);

                return $this->isRelevantForContext($relativePath) && str_ends_with($relativePath, '.php');
            })
            ->sortBy(fn (SplFileInfo $file): string => $this->relativePath($file))
            ->values();

        $lines = [
            '# PHP 符號索引',
            '',
            '產生時間：'.Carbon::now()->toDateTimeString(),
            '說明：此檔列出 PHP 檔案中的 namespace、class、method 與路由/命令符號。',
            '',
        ];

        foreach ($phpFiles as $file) {
            $relativePath = $this->relativePath($file);
            $contents = $this->files->get($file->getPathname());
            $symbols = $this->extractPhpSymbols($contents);

            if ($symbols === []) {
                continue;
            }

            $lines[] = '## '.$relativePath;

            foreach ($symbols as $symbol) {
                $lines[] = '- '.$symbol;
            }

            $lines[] = '';
        }

        return implode(PHP_EOL, $lines).PHP_EOL;
    }

    /**
     * @return array<int, string>
     */
    private function extractTestMethods(string $contents): array
    {
        preg_match_all('/public function (test_[A-Za-z0-9_]+)\s*\(/m', $contents, $matches);

        return $matches[1] ?? [];
    }

    /**
     * @return array<int, array<int, string>>
     */
    private function matchAll(string $pattern, string $contents): array
    {
        preg_match_all($pattern, $contents, $matches, PREG_SET_ORDER);

        return $matches;
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

    private function detectFileKind(string $relativePath): string
    {
        return match (true) {
            str_ends_with($relativePath, '.blade.php') => 'Blade',
            str_ends_with($relativePath, '.php') => 'PHP',
            str_ends_with($relativePath, '.json') => 'JSON',
            str_ends_with($relativePath, '.xml') => 'XML',
            str_ends_with($relativePath, '.js') => 'JavaScript',
            default => '其他',
        };
    }

    private function ownershipBucket(string $relativePath): string
    {
        return match (true) {
            str_starts_with($relativePath, 'app/Console/Commands/') => 'app: Console Commands',
            str_starts_with($relativePath, 'app/Models/') => 'app: Models',
            str_starts_with($relativePath, 'app/Providers/') => 'app: Providers',
            str_starts_with($relativePath, 'app/Http/') => 'app: HTTP',
            str_starts_with($relativePath, 'bootstrap/') => 'bootstrap',
            str_starts_with($relativePath, 'config/') => 'config',
            str_starts_with($relativePath, 'database/factories/') => 'database: factories',
            str_starts_with($relativePath, 'database/migrations/') => 'database: migrations',
            str_starts_with($relativePath, 'database/seeders/') => 'database: seeders',
            str_starts_with($relativePath, 'resources/views/') => 'resources: views',
            str_starts_with($relativePath, 'resources/js/') => 'resources: js',
            str_starts_with($relativePath, 'resources/css/') => 'resources: css',
            str_starts_with($relativePath, 'routes/') => 'routes',
            str_starts_with($relativePath, 'tests/Feature/') => 'tests: feature',
            str_starts_with($relativePath, 'tests/Unit/') => 'tests: unit',
            str_starts_with($relativePath, 'tests/') => 'tests',
            default => '其他',
        };
    }

    private function isRelevantForContext(string $relativePath): bool
    {
        if ($this->shouldExclude($relativePath)) {
            return false;
        }

        return ! str_starts_with($relativePath, 'storage/app/ai-context/');
    }

    private function formatBytes(int $bytes): string
    {
        $units = ['B', 'KB', 'MB', 'GB'];
        $value = $bytes;
        $unit = 0;

        while ($value >= 1024 && $unit < count($units) - 1) {
            $value /= 1024;
            $unit++;
        }

        return number_format($value, $unit === 0 ? 0 : 2).' '.$units[$unit];
    }

    private function shouldExclude(string $relativePath): bool
    {
        $normalizedPath = str_replace('\\', '/', $relativePath);

        foreach ([
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
        ] as $excludedPath) {
            if ($normalizedPath === $excludedPath || str_starts_with($normalizedPath, $excludedPath.'/')) {
                return true;
            }
        }

        return false;
    }

    private function matchFirst(string $pattern, string $contents): ?string
    {
        preg_match($pattern, $contents, $matches);

        return $matches[1] ?? null;
    }

    private function stringifyValue(mixed $value): string
    {
        if (is_bool($value)) {
            return $value ? 'true' : 'false';
        }

        if (is_array($value)) {
            return json_encode($value, JSON_UNESCAPED_UNICODE | JSON_UNESCAPED_SLASHES) ?: '[]';
        }

        return (string) $value;
    }

    private function relativePath(SplFileInfo $file): string
    {
        $path = str_replace('\\', '/', $file->getPathname());
        $basePath = str_replace('\\', '/', base_path());

        return ltrim(str_replace($basePath, '', $path), '/');
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
