@echo off
echo Installing Apple Music Discord RPC to startup...

set "APP_PATH=%~dp0dist\AppleMusicRPC.exe"
set "STARTUP_FOLDER=%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup"
set "SHORTCUT=%STARTUP_FOLDER%\AppleMusicRPC.lnk"

if not exist "%APP_PATH%" (
    echo Error: AppleMusicRPC.exe not found in dist folder!
    echo Please run: npm run build
    pause
    exit /b 1
)

powershell -Command "$WS = New-Object -ComObject WScript.Shell; $SC = $WS.CreateShortcut('%SHORTCUT%'); $SC.TargetPath = '%APP_PATH%'; $SC.WorkingDirectory = '%~dp0'; $SC.WindowStyle = 7; $SC.Save()"

if exist "%SHORTCUT%" (
    echo Success! Apple Music Discord RPC will now start automatically on login.
    echo Shortcut created at: %SHORTCUT%
) else (
    echo Failed to create startup shortcut.
)

pause
