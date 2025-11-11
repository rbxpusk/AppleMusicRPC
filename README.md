# Apple Music Discord RPC

Show your Apple Music tracks on Discord in real-time.

## Setup

1. Create a Discord app at https://discord.com/developers/applications
2. Copy your Application ID
3. Edit `.env` and paste your ID
4. Run `AppleMusicRPC.exe`

## Auto-start

Run `install-startup.bat` to start automatically on Windows login.

Run `uninstall-startup.bat` to remove.

## Requirements

- Windows 10/11
- Discord Desktop
- Apple Music

## Custom Images

Add artwork in your Discord app settings under Rich Presence → Art Assets:
- `apple-music-logo` (large icon)
- `play` (small icon)

## Troubleshooting

**No status showing:**
- Discord must be the desktop app, not browser
- Enable "Display current activity" in Discord Settings → Activity Privacy

**Music not detected:**
- Make sure Apple Music is running and playing

## License

MIT
