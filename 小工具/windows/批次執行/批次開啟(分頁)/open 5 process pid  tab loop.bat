@echo off
setlocal

set "PIDFILE=%~dp0workers.pid"
del "%PIDFILE%" 2>nul

rem --- 開 5 個 worker 分頁 ---
for /l %%i in (1,1,5) do (
    wt -w 0 new-tab --suppressApplicationTitle --title WORKER_%%i cmd.exe /k "%~dp0worker.cmd"
)

echo Workers started. Waiting for PID collection...

