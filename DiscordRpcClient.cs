using System;
using System.Net.Http;
using System.Threading.Tasks;
using DiscordRPC;
using DiscordRPC.Logging;
using Newtonsoft.Json.Linq;

namespace AppleMusicRPC
{
    public class DiscordRpcClient : IDisposable
    {
        private DiscordRPC.DiscordRpcClient? client;
        private readonly Settings settings;
        private readonly HttpClient httpClient;
        private string? lastArtworkUrl;

        public DiscordRpcClient(Settings settings)
        {
            this.settings = settings;
            this.httpClient = new HttpClient();
        }

        public async Task Initialize()
        {
            client = new DiscordRPC.DiscordRpcClient(settings.DiscordClientId);
            
            if (settings.DebugMode)
            {
                client.Logger = new ConsoleLogger() { Level = LogLevel.Info };
            }

            client.OnReady += (sender, e) =>
            {
                Console.WriteLine("Discord RPC connected");
                Console.WriteLine($"Logged in as: {e.User.Username}");
                Console.WriteLine("Monitoring Apple Music...");
                
                if (settings.UseAlbumArtwork)
                {
                    Console.WriteLine("Album Artwork: Enabled");
                }
                else if (!string.IsNullOrEmpty(settings.GifUrl))
                {
                    Console.WriteLine("Animated GIF: Enabled");
                }
                else
                {
                    Console.WriteLine("Using static image");
                }
                Console.WriteLine();
            };

            client.OnError += (sender, e) =>
            {
                if (settings.DebugMode)
                {
                    Console.WriteLine($"❌ Discord RPC Error: {e.Message}");
                }
            };

            if (!client.Initialize())
            {
                throw new Exception("Failed to initialize Discord RPC");
            }

            await Task.CompletedTask;
        }

        public async Task UpdatePresence(TrackInfo track, Settings settings)
        {
            if (client == null || !client.IsInitialized)
                return;

            try
            {
                string largeImageKey;
                string largeImageText = track.Album;

                // Determine image to use
                if (settings.UseAlbumArtwork)
                {
                    // Fetch album artwork from iTunes API
                    var artworkUrl = await FetchArtworkUrl(track);
                    if (!string.IsNullOrEmpty(artworkUrl))
                    {
                        largeImageKey = artworkUrl;
                        if (settings.DebugMode)
                        {
                            Console.WriteLine($"🖼️  Album artwork: {track.Album}");
                        }
                    }
                    else
                    {
                        largeImageKey = settings.GifUrl ?? "bocchi_0";
                    }
                }
                else if (!string.IsNullOrEmpty(settings.GifUrl))
                {
                    // Use external GIF URL
                    largeImageKey = settings.GifUrl;
                }
                else
                {
                    // Use static asset
                    largeImageKey = "bocchi_0";
                }

                var presence = new RichPresence
                {
                    Details = track.Name,
                    State = $"by {track.Artist}",
                    Assets = new Assets
                    {
                        LargeImageKey = largeImageKey,
                        LargeImageText = largeImageText
                    }
                };

                // Add small icon if not using external images
                if (!settings.UseAlbumArtwork && string.IsNullOrEmpty(settings.GifUrl))
                {
                    presence.Assets.SmallImageKey = "play";
                    presence.Assets.SmallImageText = "Playing";
                }

                // Add timestamps
                if (settings.ShowElapsedTime && track.Duration > 0)
                {
                    var now = DateTime.UtcNow;
                    presence.Timestamps = new Timestamps
                    {
                        Start = now.AddSeconds(-track.Position),
                        End = now.AddSeconds(track.Duration - track.Position)
                    };
                }

                client.SetPresence(presence);
            }
            catch (Exception ex)
            {
                if (settings.DebugMode)
                {
                    Console.WriteLine($"❌ Error updating presence: {ex.Message}");
                }
            }
        }

        private async Task<string?> FetchArtworkUrl(TrackInfo track)
        {
            try
            {
         
                var cleanArtist = track.Artist.Split('-')[0].Trim();
              
                var searchTerm = track.Album != "Unknown Album" 
                    ? $"{track.Album} {cleanArtist}"
                    : $"{track.Name} {cleanArtist}";

                var url = $"https://itunes.apple.com/search?term={Uri.EscapeDataString(searchTerm)}&media=music&entity=song&limit=5";
                
                var response = await httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);
                var results = json["results"] as JArray;

                if (results != null && results.Count > 0)
                {
            
                    JToken? bestMatch = results[0];
                    
                    foreach (var result in results)
                    {
                        var resultTrack = result["trackName"]?.ToString() ?? "";
                        var resultArtist = result["artistName"]?.ToString() ?? "";
                        
                        if (resultTrack.Contains(track.Name, StringComparison.OrdinalIgnoreCase) &&
                            resultArtist.Contains(cleanArtist, StringComparison.OrdinalIgnoreCase))
                        {
                            bestMatch = result;
                            break;
                        }
                    }

                    var artworkUrl = bestMatch["artworkUrl100"]?.ToString();
                    if (!string.IsNullOrEmpty(artworkUrl))
                    {
         
                        lastArtworkUrl = artworkUrl.Replace("100x100", "600x600");
                        return lastArtworkUrl;
                    }
                }
            }
            catch (Exception ex)
            {
                if (settings.DebugMode)
                {
                    Console.WriteLine($"⚠️  Artwork fetch failed: {ex.Message}");
                }
            }

            return null;
        }

        public void ClearPresence()
        {
            client?.ClearPresence();
        }

        public void Dispose()
        {
            client?.Dispose();
            httpClient?.Dispose();
        }
    }
}
