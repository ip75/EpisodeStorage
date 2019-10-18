using System;
using System.ComponentModel.DataAnnotations;

namespace Detector.EpisodeStorage.Common.Operations
{
    public class RetrieveFileOperation : Operation
    {
        [Required(ErrorMessage = "File name to retrieve is required", AllowEmptyStrings = false)]
        public string FileName { get; set; }
        [Required(ErrorMessage = "Datetime of episode creation is required")]
        public DateTime Datetime { get; set; }
        [Required(ErrorMessage = "Episode ID is required")]
        public ulong EpisodeId { get; set; }
    }
}
