@echo off
for /F %%p in (pids.txt) do (
    taskkill /PID %%p /F
)
del pids.txt
