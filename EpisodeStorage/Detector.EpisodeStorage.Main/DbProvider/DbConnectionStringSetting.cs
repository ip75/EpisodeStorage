namespace Detector.EpisodeStorage.Main.DbProvider
{
    public class DbConnectionSetting
    {
        /// <summary>
        /// the name of DbConnectionString object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the ConnectionString of DbConnectionString object.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// the provider name of DbConnectionString object.
        /// </summary>
        public string ProviderName { get; set; }
    }
}
