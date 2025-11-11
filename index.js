require('dotenv').config();
const DiscordRPC = require('discord-rpc');
const iTunesMonitor = require('./itunes-monitor');

const CLIENT_ID = process.env.DISCORD_CLIENT_ID;
const UPDATE_INTERVAL = parseInt(process.env.UPDATE_INTERVAL) || 5000;
const SHOW_ELAPSED_TIME = process.env.SHOW_ELAPSED_TIME === 'true';

if (!CLIENT_ID || CLIENT_ID === 'your_discord_client_id_here') {
  console.error('❌ Please set DISCORD_CLIENT_ID in .env file');
  console.error('   Create an app at: https://discord.com/developers/applications');
  process.exit(1);
}

const rpc = new DiscordRPC.Client({ transport: 'ipc' });
const monitor = new iTunesMonitor();
let isRpcReady = false;

async function updatePresence() {
  if (!isRpcReady) return;

  try {
    const track = await monitor.getCurrentTrack();

    if (track && monitor.hasTrackChanged(track)) {
      console.log(`🎵 Now playing: ${track.artist} - ${track.name}`);
      
      const activity = {
        details: track.name,
        state: `by ${track.artist}`,
        largeImageKey: 'apple-music-logo',
        largeImageText: track.album,
        smallImageKey: 'play',
        smallImageText: 'Playing',
        instance: false,
      };

      if (SHOW_ELAPSED_TIME && track.duration > 0) {
        const now = Date.now();
        activity.startTimestamp = now - (track.position * 1000);
        activity.endTimestamp = now + ((track.duration - track.position) * 1000);
      }

      await rpc.setActivity(activity);
      monitor.updateCurrentTrack(track);
    } else if (!track && monitor.currentTrack) {
      console.log('⏸️  Playback stopped');
      await rpc.clearActivity();
      monitor.updateCurrentTrack(null);
    }
  } catch (error) {
    console.error('Error updating presence:', error.message);
  }
}

rpc.on('ready', () => {
  console.log('✅ Discord RPC connected');
  console.log(`👤 Logged in as: ${rpc.user.username}#${rpc.user.discriminator}`);
  console.log('🎧 Monitoring Apple Music...\n');
  
  isRpcReady = true;
  updatePresence();
  setInterval(updatePresence, UPDATE_INTERVAL);
});

rpc.login({ clientId: CLIENT_ID }).catch(error => {
  console.error('❌ Failed to connect to Discord:', error.message);
  console.error('   Make sure Discord is running!');
  process.exit(1);
});

process.on('SIGINT', () => {
  console.log('\n👋 Shutting down...');
  monitor.cleanup();
  rpc.clearActivity();
  rpc.destroy();
  process.exit(0);
});

process.on('exit', () => {
  monitor.cleanup();
});
