@echo off
rem powershell -NoProfile -Command "$PID" >> "%~dp0workers.pid"



cd /d "D:\web\web\hahaha_base\hahaha\hahaha"

rem 讓 php.exe 取代 cmd.exe（cmd.exe 結束 → tab 自動關閉）
cmd /c php artisan queue:work --tries=3