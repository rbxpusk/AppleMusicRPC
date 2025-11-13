# Apple Music Discord RPC

Show what you're listening to on Apple Music directly in your Discord status with real-time updates and animated artwork.

## Features

- Real-time track updates
- Animated GIF support for custom artwork
- Automatic album artwork from iTunes
- Playback progress display
- Auto-start on Windows login
- Lightweight and fast

## Requirements

- Windows 10/11
- .NET 6.0 Runtime ([Download](https://dotnet.microsoft.com/download/dotnet/6.0))
- Discord Desktop
- Apple Music

## Quick Start

1. Download the latest release
2. Create a Discord application at https://discord.com/developers/applications
3. Copy your Application ID
4. Edit `.env` and paste your ID
5. Run `AppleMusicRPC.exe`
6. Play music in Apple Music

Your Discord status will update automatically!

## Configuration

Edit `.env` to customize:

```env
DISCORD_CLIENT_ID=your_app_id_here
UPDATE_INTERVAL=5000
SHOW_ELAPSED_TIME=true
USE_ALBUM_ARTWORK=true
GIF_URL=
DEBUG_MODE=false
```

### Image Options

**Album Artwork** (Recommended)
```env
USE_ALBUM_ARTWORK=true
```
Automatically fetches album covers from iTunes.

**Animated GIF**
```env
USE_ALBUM_ARTWORK=false
GIF_URL=https://i.imgur.com/yourfile.gif
```
Use any animated GIF from Imgur, Tenor, Giphy, etc.

**Static Image**
```env
USE_ALBUM_ARTWORK=false
GIF_URL=
```
Uses a static image asset uploaded to Discord.

## Auto-Start

Run `install-startup.bat` to start automatically when Windows boots.
Run `uninstall-startup.bat` to remove from startup.

## Building from Source

```bash
dotnet build
dotnet run
```

Or build a release:
```bash
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o release
```

## Troubleshooting

**No status showing?**
- Make sure Discord desktop app is running
- Enable "Display current activity" in Discord Settings
- Verify your Application ID is correct

**Music not detected?**
- Ensure Apple Music is running and playing
- Try restarting the app

**Album artwork not loading?**
- Check your internet connection
- Some songs may not have artwork available

## License

MIT
