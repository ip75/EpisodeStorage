namespace Detector.EpisodeStorage.Common
{
    /// <summary>
    /// Config
    /// </summary>
    public class Config
    {
        public string RootDirectory { get; set; }
        public bool CleanObsoleteFiles { get; set; }
        public bool SignEnabled { get; set; }
        public string KeyPath { get; set; }
        public int KeepPeriod { get; set; }

        /// <summary>
        /// Transceiver
        /// </summary>
        public string ZeroMQRouter { get; set; }
        public int TimeoutMessageProcessing { get; set; }

        /// <summary>
        /// Lightener section
        /// </summary>
        public bool LighterEnabled { get; set; }
        public int JpegQuality { get; set; }
        public double Brightness { get; set; }
        public double Locality { get; set; }
        public double ContrastPlus { get; set; }
        public bool BrightnessEquAlgorithm  { get; set; }
        public double Saturation  { get; set; }
        public bool CompressForLighter { get; set; }
        public double Radius { get; set; }
        public double Weight { get; set; }

        public override string ToString()
        {
            return $"RootDirectory: {RootDirectory}, CleanObsoleteFiles: {CleanObsoleteFiles}, LighterEnabled: {LighterEnabled}, JpegQuality:{JpegQuality}";
        }
    }
}
