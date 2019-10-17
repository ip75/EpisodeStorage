using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Detector.EpisodeStorage.ScreenShotDB
{
    public class EpisodeDirectory
    {
        public ulong EpisodeId;
        public DirectoryInfo Directory;
        public async Task StoreFile(string fileName, Stream data)
        {
            await using var fs = new FileStream(Path.Combine(Directory.FullName, fileName), FileMode.CreateNew, FileAccess.Write);
            await data.CopyToAsync(fs);
        }
        public async Task StoreFile(string fileName, byte [] data)
        {
            await using var file = File.Open(Path.Combine(Directory.FullName, fileName), FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.End);
            await file.WriteAsync(data);
        }
        public async Task StoreFile(string fileName, string data)
        {
            await using var file = File.Open(Path.Combine(Directory.FullName, fileName), FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.End);
            await file.WriteAsync(Encoding.UTF8.GetBytes(data));
        }
    }
}
