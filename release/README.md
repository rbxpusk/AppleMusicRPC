# Apple Music Discord RPC

Display your Apple Music tracks on Discord with animated artwork.

## Setup

1. Go to https://discord.com/developers/applications
2. Create a new application
3. Copy your Application ID
4. Open `.env` and replace `YOUR_APP_ID` with your ID
5. Run `AppleMusicRPC.exe`
6. Play music in Apple Music

## Features

- Real-time track updates
- Animated GIF support
- Automatic album artwork
- Playback progress
- Auto-start on login

## Configuration

Edit `.env` to customize settings.

**Album Artwork**
```
USE_ALBUM_ARTWORK=true
```

**Animated GIF**
```
USE_ALBUM_ARTWORK=false
GIF_URL=https://i.imgur.com/yourfile.gif
```

## Auto-Start

Run `install-startup.bat` to start with Windows.
Run `uninstall-startup.bat` to remove.

## Troubleshooting

**No status showing?**
- Use Discord desktop app, not browser
- Enable "Display current activity" in Discord Settings
- Check your Application ID

**Music not detected?**
- Make sure Apple Music is running and playing
- Try restarting the app

## Requirements

- Windows 10/11
- .NET 6.0 Runtime
- Discord Desktop
- Apple Music

## License

MIT
