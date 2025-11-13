@echo off
echo Building Apple Music Discord RPC...
echo.

dotnet --version >nul 2>&1
if errorlevel 1 (
    echo .NET SDK not found. Please install .NET 6.0 SDK.
    echo Download: https://dotnet.microsoft.com/download/dotnet/6.0
    pause
    exit /b 1
)

dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o release

if errorlevel 1 (
    echo Build failed!
    pause
    exit /b 1
)

echo.
echo Build complete: release\AppleMusicRPC.exe
pause
