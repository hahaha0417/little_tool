@echo off
setlocal enabledelayedexpansion

:: ============================================
:: 純字串
:: set "AddPath=%%hahaha_bin%%"
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

set "TmpPath=%UserPath%"
set "NewPath="

:: ====== 精準去重 ======
:: 先檢查 AddPath 是否已存在
echo ;%TmpPath%; | findstr /I /C:";%AddPath%;" >nul
if errorlevel 1 (
    echo 新項目不存在，加入。
    set "TmpPath=%TmpPath%;%AddPath%"
) else (
    echo 新項目已存在，不加入。
)

:: ====== 重新整理 PATH（去除重複、空白） ======
set "rest=%TmpPath%"
:loop
for /f "delims=;" %%A in ("!rest!") do (
    set "item=%%A"
)

set "rest=!rest:*;=!"

if not "!item!"=="" (
    echo ;!NewPath!; | findstr /I /C:";!item!;" >nul
    if errorlevel 1 (
        if defined NewPath (
            set "NewPath=!NewPath!;!item!"
        ) else (
            set "NewPath=!item!"
        )
    )
)

if not "!rest!"=="!item!" goto loop

echo.
echo [3] 最終使用者 PATH:
echo !NewPath!
echo.

echo [4] 寫回使用者環境變數...
reg add "HKCU\Environment" /v PATH /t REG_EXPAND_SZ /d "!NewPath!" /f >nul

echo 完成！
echo ※ 記得重新開啟 CMD 或重登才會生效。



