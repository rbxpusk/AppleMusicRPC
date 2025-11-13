namespace AppleMusicRPC
{
    public class TrackInfo
    {
        public string Name { get; set; } = "";
        public string Artist { get; set; } = "";
        public string Album { get; set; } = "";
        public int Duration { get; set; }
        public int Position { get; set; }
        public bool IsPlaying { get; set; }
    }
}
