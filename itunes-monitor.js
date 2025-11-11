const { exec } = require('child_process');
const { promisify } = require('util');
const fs = require('fs');
const path = require('path');
const os = require('os');

const execAsync = promisify(exec);

class AppleMusicMonitor {
  constructor() {
    this.currentTrack = null;
    this.tempScriptPath = path.join(os.tmpdir(), 'apple-music-rpc-' + Date.now() + '.ps1');
    
    const psScript = `try {
  Add-Type -AssemblyName System.Runtime.WindowsRuntime
  $asTaskGeneric = ([System.WindowsRuntimeSystemExtensions].GetMethods() | Where-Object { $_.Name -eq 'AsTask' -and $_.GetParameters().Count -eq 1 -and $_.GetParameters()[0].ParameterType.Name -eq 'IAsyncOperation\`1' })[0]
  
  Function Await($WinRtTask, $ResultType) {
    $asTask = $asTaskGeneric.MakeGenericMethod($ResultType)
    $netTask = $asTask.Invoke($null, @($WinRtTask))
    $netTask.Wait(-1) | Out-Null
    $netTask.Result
  }

  [Windows.Media.Control.GlobalSystemMediaTransportControlsSessionManager, Windows.Media.Control, ContentType = WindowsRuntime] | Out-Null
  $sessionManager = Await ([Windows.Media.Control.GlobalSystemMediaTransportControlsSessionManager]::RequestAsync()) ([Windows.Media.Control.GlobalSystemMediaTransportControlsSessionManager])
  
  $sessions = $sessionManager.GetSessions()
  $appleSession = $sessions | Where-Object { $_.SourceAppUserModelId -like '*AppleMusic*' }
  
  if ($appleSession) {
    $mediaProps = Await ($appleSession.TryGetMediaPropertiesAsync()) ([Windows.Media.Control.GlobalSystemMediaTransportControlsSessionMediaProperties])
    $playbackInfo = $appleSession.GetPlaybackInfo()
    $timelineProps = $appleSession.GetTimelineProperties()
    
    if ($playbackInfo.PlaybackStatus -eq 'Playing') {
      $name = if ($mediaProps.Title) { $mediaProps.Title } else { "Unknown" }
      $artist = if ($mediaProps.Artist) { $mediaProps.Artist } else { "Unknown Artist" }
      $album = if ($mediaProps.AlbumTitle) { $mediaProps.AlbumTitle } else { "Unknown Album" }
      
      $duration = [Math]::Floor($timelineProps.EndTime.TotalSeconds)
      $position = [Math]::Floor($timelineProps.Position.TotalSeconds)
      
      Write-Output "PLAYING|$name|$artist|$album|$duration|$position"
    } else {
      Write-Output "STOPPED"
    }
  } else {
    Write-Output "STOPPED"
  }
} catch {
  Write-Output "ERROR|$($_.Exception.Message)"
}`;
    
    fs.writeFileSync(this.tempScriptPath, psScript, 'utf8');
  }

  async getCurrentTrack() {
    try {
      const { stdout } = await execAsync(
        `powershell -NoProfile -ExecutionPolicy Bypass -File "${this.tempScriptPath}"`,
        { timeout: 5000, windowsHide: true }
      );

      const out = stdout.trim();

      if (out.startsWith('PLAYING|')) {
        const [_, name, artist, album, duration, position] = out.split('|');
        return {
          name: name || 'Unknown',
          artist: artist || 'Unknown Artist',
          album: album || 'Unknown Album',
          duration: Math.floor(parseFloat(duration) || 0),
          position: Math.floor(parseFloat(position) || 0),
          isPlaying: true
        };
      }
      return null;
    } catch (err) {
      return null;
    }
  }

  hasTrackChanged(newTrack) {
    if (!newTrack && !this.currentTrack) return false;
    if (!newTrack || !this.currentTrack) return true;

    return (
      newTrack.name !== this.currentTrack.name ||
      newTrack.artist !== this.currentTrack.artist ||
      newTrack.isPlaying !== this.currentTrack.isPlaying
    );
  }

  updateCurrentTrack(track) {
    this.currentTrack = track;
  }

  cleanup() {
    try {
      if (fs.existsSync(this.tempScriptPath)) {
        fs.unlinkSync(this.tempScriptPath);
      }
    } catch (err) {}
  }
}

module.exports = AppleMusicMonitor;
