@echo off
setlocal
set "PIDFILE=%~dp0pids.txt"
del "%PIDFILE%" 2>nul

for /L %%i in (1,1,5) do (
    powershell -NoProfile -Command ^
    "$cmd = 'cmd.exe /k ""title WORKER_%%i & echo hahaha %%i & pause""';" ^
    "$p = Start-Process -FilePath cmd.exe -ArgumentList '/k', 'title WORKER_%%i & echo hahaha %%i & pause' -PassThru;" ^
    "$p.Id | Out-File '%PIDFILE%' -Append -Encoding ascii"
)

pause
