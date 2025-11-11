@echo off
echo ========================================
echo Apple Music Discord RPC - Release Build
echo ========================================

echo.
echo [1/5] Cleaning old build...
if exist release rmdir /s /q release
if exist dist rmdir /s /q dist

echo.
echo [2/5] Installing dependencies...
call npm install

echo.
echo [3/5] Building executable...
call npx @yao-pkg/pkg . --targets node18-win-x64 --output dist/AppleMusicRPC.exe

echo.
echo [4/5] Creating release package...
mkdir release
copy dist\AppleMusicRPC.exe release\
copy .env release\.env.example
copy README.md release\
copy LICENSE release\ 2>nul
copy install-startup.bat release\
copy uninstall-startup.bat release\

echo.
echo [5/5] Creating instructions...
(
echo APPLE MUSIC DISCORD RPC
echo =======================
echo.
echo SETUP:
echo 1. Edit .env.example and add your Discord Client ID
echo 2. Rename .env.example to .env
echo 3. Run AppleMusicRPC.exe
echo.
echo AUTO-START ON WINDOWS LOGIN:
echo - Run install-startup.bat
echo.
echo TO REMOVE FROM STARTUP:
echo - Run uninstall-startup.bat
echo.
echo Get your Discord Client ID:
echo https://discord.com/developers/applications
) > release\INSTRUCTIONS.txt

echo.
echo ========================================
echo Release package created in 'release' folder!
echo ========================================
echo.
echo Contents:
dir /b release
echo.
pause
