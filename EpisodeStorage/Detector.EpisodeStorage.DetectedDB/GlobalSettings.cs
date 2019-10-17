namespace Detector.EpisodeStorage.DetectedDB
{
    public class GlobalSettings
    {
        private ulong _episodeId;

        public ulong EpisodeId
        {
            get => _episodeId++;
            set => _episodeId = value;
        }
    }
}
