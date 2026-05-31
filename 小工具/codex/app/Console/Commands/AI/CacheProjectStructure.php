<?php

namespace App\Console\Commands\AI;

use Illuminate\Console\Command;
use Illuminate\Console\Attributes\Description;
use Illuminate\Console\Attributes\Signature;
use Illuminate\Filesystem\Filesystem;
use Illuminate\Support\Carbon;

#[Signature('app:cache-project-structure {--output= : Output file path}')]
#[Description('為 AI 助手快取可讀的專案結構快照')]
class CacheProjectStructure extends Command
{
    private const DEFAULT_OUTPUT = '.codex/project-structure.md';

    private const EXCLUDED_PATHS = [
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

    public function __construct(
        private readonly Filesystem $files,
    ) {
        parent::__construct();
    }

    public function handle(): int
    {
        $outputPath = $this->resolveOutputPath((string) $this->option('output'));
        $entries = $this->scanDirectory(base_path());
        $contents = $this->renderSnapshot($entries);

        $this->files->replace($outputPath, $contents);

        $this->components->info(sprintf('專案結構已快取到 %s', $this->displayPath($outputPath)));

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
     * @return array<int, array{name: string, path: string, directory: bool, children: array<int, array{name: string, path: string, directory: bool, children: array}>}>
     */
    private function scanDirectory(string $path, string $relativePath = ''): array
    {
        $entries = [];
        $directoryEntries = scandir($path) ?: [];

        foreach ($directoryEntries as $entry) {
            if ($entry === '.' || $entry === '..') {
                continue;
            }

            $entryRelativePath = $relativePath === '' ? $entry : $relativePath.'/'.$entry;

            if ($this->shouldExclude($entryRelativePath)) {
                continue;
            }

            $fullPath = $path.DIRECTORY_SEPARATOR.$entry;
            $isDirectory = is_dir($fullPath);

            $entries[] = [
                'name' => $entry,
                'path' => $entryRelativePath,
                'directory' => $isDirectory,
                'children' => $isDirectory ? $this->scanDirectory($fullPath, $entryRelativePath) : [],
            ];
        }

        usort(
            $entries,
            static function (array $left, array $right): int {
                if ($left['directory'] !== $right['directory']) {
                    return $left['directory'] ? -1 : 1;
                }

                return strcasecmp($left['name'], $right['name']);
            },
        );

        return $entries;
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

    /**
     * @param array<int, array{name: string, path: string, directory: bool, children: array<int, array{name: string, path: string, directory: bool, children: array}>}> $entries
     */
    private function renderSnapshot(array $entries): string
    {
        $lines = [
            '# 專案結構快照',
            '',
            '產生時間：'.Carbon::now()->toDateTimeString(),
            '根目錄：`'.base_path().'`',
            '已忽略：`'.implode('`, `', self::EXCLUDED_PATHS).'`',
            '',
            '```text',
            '.',
            ...$this->renderTree($entries),
            '```',
            '',
        ];

        return implode(PHP_EOL, $lines).PHP_EOL;
    }

    /**
     * @param array<int, array{name: string, path: string, directory: bool, children: array<int, array{name: string, path: string, directory: bool, children: array}>}> $entries
     * @return array<int, string>
     */
    private function renderTree(array $entries, string $prefix = ''): array
    {
        $lines = [];
        $totalEntries = count($entries);

        foreach ($entries as $index => $entry) {
            $isLastEntry = $index === $totalEntries - 1;
            $connector = $isLastEntry ? '\-- ' : '|-- ';
            $suffix = $entry['directory'] ? '/' : '';

            $lines[] = $prefix.$connector.$entry['name'].$suffix;

            if ($entry['directory'] && $entry['children'] !== []) {
                $childPrefix = $prefix.($isLastEntry ? '    ' : '|   ');
                $lines = array_merge($lines, $this->renderTree($entry['children'], $childPrefix));
            }
        }

        return $lines;
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
