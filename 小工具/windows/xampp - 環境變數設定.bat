@echo off
setlocal enabledelayedexpansion

:: ============================================
:: 要加入的路徑（修改這裡！）
set "AddPath=D:\web\xampp\mysql\bin;D:\web\xampp\php"
:: ============================================

echo [1] 讀取使用者 PATH...

set "UserPath="

for /f "tokens=2* delims= " %%A in ('reg query "HKCU\Environment" /v PATH 2^>nul') do (
    set "UserPath=%%B"
)

if not defined UserPath (
    echo 使用者 PATH 不存在，建立空的。
    set "UserPath="
)

echo 原始使用者 PATH:
echo %UserPath%
echo.

echo [2] 處理並加入新項目...

set "TmpPath=%UserPath%;%AddPath%"

set "NewPath="

for %%A in ("%TmpPath:;=" "%") do (
    set "item=%%~A"
    if not "!item!"=="" (
        echo !NewPath! | findstr /I /C:"!item!" >nul
        if errorlevel 1 (
            if defined NewPath (
                set "NewPath=!NewPath!;!item!"
            ) else (
                set "NewPath=!item!"
            )
        )
    )
)

echo.
echo [3] 最終使用者 PATH:
echo !NewPath!
echo.

echo [4] 寫回使用者環境變數...
reg add "HKCU\Environment" /v PATH /t REG_EXPAND_SZ /d "!NewPath!" /f >nul

echo 完成！
echo ※ 記得重新開啟 CMD 或重登才會生效。
