using System.ComponentModel.DataAnnotations;

namespace Detector.EpisodeStorage.Common.Operations
{
    public class StoreFileOperation : Operation
    {
        [Required(ErrorMessage = "File name to retrieve is required", AllowEmptyStrings = false)]
        public string FileName { get; set; }
        public byte [] Data { get; set; }
    }
}
