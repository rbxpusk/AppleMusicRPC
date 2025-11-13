using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AppleMusicRPC
{
    public class AppleMusicMonitor : IDisposable
    {
        private const string PowerShellScript = @"try {
    Add-Type -AssemblyName System.Runtime.WindowsRuntime
    $asTaskGeneric = ([System.WindowsRuntimeSystemExtensions].GetMethods() | Where-Object { $_.Name -eq 'AsTask' -and $_.GetParameters().Count -eq 1 -and $_.GetParameters()[0].ParameterType.Name -eq 'IAsyncOperation`1' })[0]
    
    Function Await($WinRtTask, $ResultType) {
        $asTask = $asTaskGeneric.MakeGenericMethod($ResultType)
        $netTask = $asTask.Invoke($null, @($WinRtTask))
        $netTask.Wait(-1) | Out-Null
        $netTask.Result
    }

    [Windows.Media.Control.GlobalSystemMediaTransportControlsSessionManager, Windows.Media.Control, ContentType = WindowsRuntime] | Out-Null
    $sessionManager = Await ([Windows.Media.Control.GlobalSystemMediaTransportControlsSessionManager]::RequestAsync()) ([Windows.Media.Control.GlobalSystemMediaTransportControlsSessionManager])
    
    $sessions = $sessionManager.GetSessions()
    $appleSession = $sessions | Where-Object { $_.SourceAppUserModelId -like '*Apple*' }
    
    if ($appleSession) {
        $mediaProps = Await ($appleSession.TryGetMediaPropertiesAsync()) ([Windows.Media.Control.GlobalSystemMediaTransportControlsSessionMediaProperties])
        $playbackInfo = $appleSession.GetPlaybackInfo()
        $timelineProps = $appleSession.GetTimelineProperties()
        
        if ($playbackInfo.PlaybackStatus -eq 'Playing') {
            $name = if ($mediaProps.Title) { $mediaProps.Title } else { 'Unknown' }
            $artist = if ($mediaProps.Artist) { $mediaProps.Artist } else { 'Unknown Artist' }
            $album = if ($mediaProps.AlbumTitle) { $mediaProps.AlbumTitle } else { 'Unknown Album' }
            
            $duration = [Math]::Floor($timelineProps.EndTime.TotalSeconds)
            $position = [Math]::Floor($timelineProps.Position.TotalSeconds)
            
            Write-Output ('PLAYING|' + $name + '|' + $artist + '|' + $album + '|' + $duration + '|' + $position)
        } else {
            Write-Output 'STOPPED'
        }
    } else {
        Write-Output 'STOPPED'
    }
} catch {
    Write-Output ('ERROR|' + $_.Exception.Message)
}";

        private readonly string scriptPath;

        public AppleMusicMonitor()
        {
            // Save script to temp file
            scriptPath = Path.Combine(Path.GetTempPath(), "apple-music-monitor.ps1");
            File.WriteAllText(scriptPath, PowerShellScript);
        }

        public async Task<TrackInfo?> GetCurrentTrack()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                if (process == null) return null;

                var output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                output = output.Trim();

                if (output.StartsWith("PLAYING|"))
                {
                    var parts = output.Split('|');
                    if (parts.Length >= 6)
                    {
                        return new TrackInfo
                        {
                            Name = parts[1],
                            Artist = parts[2],
                            Album = parts[3],
                            Duration = int.TryParse(parts[4], out int dur) ? dur : 0,
                            Position = int.TryParse(parts[5], out int pos) ? pos : 0,
                            IsPlaying = true
                        };
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public void Dispose()
        {
            try
            {
                if (File.Exists(scriptPath))
                {
                    File.Delete(scriptPath);
                }
            }
            catch { }
        }
    }
}
