using System;

namespace Detector.Common
{
    public interface ICleaner
    {
        /// <summary>
        /// Keep files or records while this period, else remove them all
        /// </summary>
        TimeSpan KeepPeriod { get; set; }
        void Clean();
    }
}
