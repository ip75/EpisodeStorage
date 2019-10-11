namespace Detector.EpisodeStorage.Main.Operations
{
    public class StoreFileOperation : Operation
    {
        public string FileName { get; set; }
        public byte [] Data { get; set; }
    }
}
