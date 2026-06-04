$ErrorActionPreference = 'Stop'

$Repository_Url_ = 'https://github.com/hahaha0417/php_hahaha_console.git'
$Target_Directory_ = 'hahaha_console'
$Target_Path_ = Join-Path -Path (Get-Location) -ChildPath $Target_Directory_

if (Test-Path -LiteralPath $Target_Path_) {
    Write-Host "Target directory already exists: $Target_Path_"
} else {
    git clone $Repository_Url_ $Target_Directory_
}

$Repository_Url_Hahaha_Laravel_Lib_ = 'https://github.com/hahaha0417/php_hahaha_laravel_lib.git'
$Target_Directory_Hahaha_Laravel_Lib_ = 'library/hahaha_laravel_lib'
$Target_Path_Hahaha_Laravel_Lib_ = Join-Path -Path $Target_Path_ -ChildPath $Target_Directory_Hahaha_Laravel_Lib_

if (Test-Path -LiteralPath $Target_Path_Hahaha_Laravel_Lib_) {
    Write-Host "Target directory already exists: $Target_Path_Hahaha_Laravel_Lib_"
} else {
    New-Item -ItemType Directory -Path (Split-Path $Target_Path_Hahaha_Laravel_Lib_) -Force | Out-Null
    git clone $Repository_Url_Hahaha_Laravel_Lib_ $Target_Path_Hahaha_Laravel_Lib_
}

Copy-Item ./.env $Target_Path_

Set-Location -LiteralPath $Target_Path_

composer install

php artisan app:hahaha-cache-ai-context
php artisan app:hahaha-cache-code-summary
php artisan app:hahaha-cache-project-structure