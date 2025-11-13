# Troubleshooting Guide

## GIF Animation Not Working

### Step 1: Enable Debug Mode

Edit your `.env` file and set:
```env
DEBUG_MODE=true
GIF_ANIMATION_ENABLED=true
```

Run the app and check the console output for detailed logs.

### Step 2: Verify Your Settings

Check your `.env` file has the correct values:

```env
# Make sure this matches your Discord app ID
DISCORD_CLIENT_ID=1437777570316947496

# Enable GIF animation
GIF_ANIMATION_ENABLED=true

# This must match your Discord asset names
GIF_FRAME_PREFIX=bocchi

# Count ALL frames you uploaded (0 to 19 = 20 frames)
GIF_FRAME_COUNT=20

# Animation speed (lower = faster)
GIF_FRAME_DELAY=100
```

### Step 3: Check Discord Assets

1. Go to https://discord.com/developers/applications
2. Select your app → Rich Presence → Art Assets
3. Verify you have ALL frames uploaded:
   - `bocchi_0`, `bocchi_1`, `bocchi_2`, ... `bocchi_19`
4. Make sure there are NO gaps in the sequence

**Currently you're missing:** bocchi_4, bocchi_5, bocchi_6, bocchi_7, bocchi_8, bocchi_9, bocchi_11, bocchi_15, bocchi_17

### Step 4: Common Errors

**Error: "Unknown Asset"**
- This means Discord can't find the image
- Check the asset name matches exactly: `bocchi_0` not `bocchi_00` or `Bocchi_0`
- Make sure the frame number exists in Discord
- Wait a few minutes after uploading - Discord needs time to process assets

**Animation not starting:**
- Make sure `GIF_ANIMATION_ENABLED=true` (not "True" or "TRUE")
- Check that music is actually playing in Apple Music
- Verify Discord desktop app is running (not browser)

**Animation too fast/slow:**
- Adjust `GIF_FRAME_DELAY` value
- 50ms = 20fps (very fast)
- 100ms = 10fps (normal)
- 200ms = 5fps (slow)

**Only first frame shows:**
- Check console for "Unknown Asset" errors
- This usually means missing frames in Discord
- Upload ALL frames from bocchi_0 to bocchi_19

### Step 5: Test with Static Image First

If GIF still doesn't work, test with a static image:

```env
GIF_ANIMATION_ENABLED=false
```

Make sure you have `apple-music-logo` uploaded to Discord. If this works, the issue is with your GIF frames.

### Step 6: Verify Frame Count

Count your uploaded Discord assets:
- If you have bocchi_0 through bocchi_19, that's 20 frames
- Set `GIF_FRAME_COUNT=20`
- The count must match EXACTLY

### Debug Output Example

When `DEBUG_MODE=true`, you should see:
```
✅ Discord RPC connected
👤 Logged in as: YourName#0000
🎧 Monitoring Apple Music...
🎬 GIF Animation: ENABLED
   Prefix: bocchi
   Frames: 20 (bocchi_0 to bocchi_19)
   Delay: 100ms (10.0 fps)
🐛 DEBUG MODE: Enabled

🎵 Now playing: Artist - Song Name
[DEBUG] Using image key: bocchi_0
[DEBUG] Setting activity: {...}
[DEBUG] ✓ Activity set successfully
🎬 Starting GIF animation (20 frames @ 100ms)
[DEBUG] Frame update: bocchi_1
[DEBUG] ✓ Frame updated: bocchi_1
[DEBUG] Frame update: bocchi_2
...
```

If you see errors like:
```
❌ Frame update failed (bocchi_4): Unknown Asset
   Missing asset: bocchi_4
   Upload this frame to Discord Developer Portal!
```

This tells you exactly which frame is missing!

## Still Not Working?

1. Make sure you uploaded ALL 20 frames (no gaps)
2. Wait 5 minutes after uploading for Discord to process
3. Restart the app
4. Check Discord Developer Portal shows all assets
5. Try with a smaller frame count first (like 5 frames) to test
