using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppleMusicRPC
{
    class Program
    {
        private static AppleMusicMonitor? monitor;
        private static DiscordRpcClient? rpcClient;
        private static CancellationTokenSource? cts;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Apple Music Discord RPC v2.0.0");
            Console.WriteLine("================================\n");

            // Load settings
            var settings = Settings.Load();
            
            if (string.IsNullOrEmpty(settings.DiscordClientId) || settings.DiscordClientId == "YOUR_APP_ID")
            {
                Console.WriteLine("Please set DISCORD_CLIENT_ID in .env file");
                Console.WriteLine("Create an app at: https://discord.com/developers/applications");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            // Initialize
            monitor = new AppleMusicMonitor();
            rpcClient = new DiscordRpcClient(settings);
            cts = new CancellationTokenSource();

            // Handle Ctrl+C
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                cts?.Cancel();
            };

            try
            {
                await rpcClient.Initialize();
                await RunUpdateLoop(settings, cts.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nShutting down...");
                rpcClient?.Dispose();
                monitor?.Dispose();
            }
        }

        static async Task RunUpdateLoop(Settings settings, CancellationToken cancellationToken)
        {
            TrackInfo? lastTrack = null;
            bool firstCheck = true;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var currentTrack = await monitor!.GetCurrentTrack();

                    if (firstCheck)
                    {
                        if (currentTrack == null)
                        {
                            Console.WriteLine("Waiting for Apple Music...");
                        }
                        firstCheck = false;
                    }

                    if (currentTrack != null && HasTrackChanged(currentTrack, lastTrack))
                    {
                        Console.WriteLine($"Now playing: {currentTrack.Artist} - {currentTrack.Name}");
                        await rpcClient!.UpdatePresence(currentTrack, settings);
                        lastTrack = currentTrack;
                    }
                    else if (currentTrack == null && lastTrack != null)
                    {
                        Console.WriteLine("Playback stopped");
                        rpcClient!.ClearPresence();
                        lastTrack = null;
                    }

                    await Task.Delay(settings.UpdateInterval, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error: {ex.Message}");
                    if (settings.DebugMode)
                    {
                        Console.WriteLine($"   Stack: {ex.StackTrace}");
                    }
                    await Task.Delay(1000, cancellationToken);
                }
            }
        }

        static bool HasTrackChanged(TrackInfo current, TrackInfo? last)
        {
            if (last == null) return true;
            return current.Name != last.Name || 
                   current.Artist != last.Artist || 
                   current.Album != last.Album;
        }
    }
}
