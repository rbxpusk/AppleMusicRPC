@echo off
echo Removing Apple Music Discord RPC from startup...

set "STARTUP_FOLDER=%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup"
set "SHORTCUT=%STARTUP_FOLDER%\AppleMusicRPC.lnk"

if exist "%SHORTCUT%" (
    del "%SHORTCUT%"
    echo Success! Apple Music Discord RPC removed from startup.
) else (
    echo Shortcut not found in startup folder.
)

pause
