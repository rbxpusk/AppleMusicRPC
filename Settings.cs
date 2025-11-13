using System;
using System.IO;
using System.Collections.Generic;

namespace AppleMusicRPC
{
    public class Settings
    {
        public string DiscordClientId { get; set; } = "YOUR_APP_ID";
        public int UpdateInterval { get; set; } = 5000;
        public bool ShowElapsedTime { get; set; } = true;
        public bool UseAlbumArtwork { get; set; } = false;
        public string GifUrl { get; set; } = "";
        public bool DebugMode { get; set; } = false;

        public static Settings Load()
        {
            var settings = new Settings();
            var envPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");

            if (!File.Exists(envPath))
            {
                Console.WriteLine("  .env file not found, using defaults");
                return settings;
            }

            try
            {
                var lines = File.ReadAllLines(envPath);
                var env = new Dictionary<string, string>();

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                        continue;

                    var parts = line.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        env[parts[0].Trim()] = parts[1].Trim();
                    }
                }

                if (env.ContainsKey("DISCORD_CLIENT_ID"))
                    settings.DiscordClientId = env["DISCORD_CLIENT_ID"];

                if (env.ContainsKey("UPDATE_INTERVAL") && int.TryParse(env["UPDATE_INTERVAL"], out int interval))
                    settings.UpdateInterval = interval;

                if (env.ContainsKey("SHOW_ELAPSED_TIME"))
                    settings.ShowElapsedTime = env["SHOW_ELAPSED_TIME"].ToLower() == "true";

                if (env.ContainsKey("USE_ALBUM_ARTWORK"))
                    settings.UseAlbumArtwork = env["USE_ALBUM_ARTWORK"].ToLower() == "true";

                if (env.ContainsKey("GIF_URL"))
                    settings.GifUrl = env["GIF_URL"];

                if (env.ContainsKey("DEBUG_MODE"))
                    settings.DebugMode = env["DEBUG_MODE"].ToLower() == "true";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading .env: {ex.Message}");
            }

            return settings;
        }
    }
}
