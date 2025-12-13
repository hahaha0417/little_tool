@echo off
setlocal EnableDelayedExpansion

set "PIDFILE=%~dp0wt_pid.txt"
del "%PIDFILE%" 2>nul

powershell -NoProfile -Command ^
  "$args=@();" ^
  "for($i=1;$i -le 5;$i++){" ^
  "  if($i -gt 1){$args+=';'}" ^
  "  $args+=@('new-tab','--title',('WORKER_{0}' -f $i),'cmd.exe','/k',('echo hahaha {0} & pause' -f $i))" ^
  "}" ^
  "$p=Start-Process -FilePath 'wt.exe' -ArgumentList $args -PassThru;" ^
  "$p.Id | Out-File -FilePath '%PIDFILE%' -Encoding ascii"

echo WT PID saved to "%PIDFILE%"
pause
