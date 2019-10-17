namespace Detector.EpisodeStorage.Common.Operations
{
    public class StoreFileOperation : Operation
    {
        public string FileName { get; set; }
        public byte [] Data { get; set; }
    }
}
