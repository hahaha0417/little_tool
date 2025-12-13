@echo off
setlocal EnableDelayedExpansion

set "cmdline=new-tab --title WORKER_1 cmd /k ""echo hahaha 1 ^& pause"""

for /L %%i in (2,1,5) do (
  set "cmdline=!cmdline! ; new-tab --title WORKER_%%i cmd /k ""echo hahaha %%i ^& pause"""
)

wt !cmdline!
