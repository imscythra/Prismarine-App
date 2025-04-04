using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using System.IO;
using System.Diagnostics;

namespace Prismarine.Helpers
{
    public class VideoHelper
    {
        public string FileName { get; set; } = string.Empty;
        public string? ThumbnailPath { get; set; }
        public string? Duration { get; set; }
        public DateTime DateModified { get; set; }
    }
    public class VideoManager
    {
        public ObservableCollection<VideoHelper> Videos { get; set; } = new();

        public async Task LoadVideos()
        {
            await Task.Delay(500); // Wait 500ms before accessing files

            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "Prismarine");

            if (!Directory.Exists(folderPath)) return;

            var files = Directory.GetFiles(folderPath, "*.*")
                .Where(f => f.EndsWith(".mp4") || f.EndsWith(".mp3"))
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.LastWriteTime)
                .ToList();

            Videos.Clear();
            foreach (var file in files)
            {
                try
                {

                    StorageFile storageFile = await StorageFile.GetFileFromPathAsync(file.FullName);
                    var properties = await storageFile.Properties.GetVideoPropertiesAsync();
                    Videos.Add(new VideoHelper
                    {
                        FileName = file.Name,
                        Duration = properties.Duration.ToString(@"hh\:mm\:ss"),
                        DateModified = file.LastWriteTime,
                        ThumbnailPath = await GenerateThumbnail(storageFile)
                    });
                    Debug.WriteLine(file.Name);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }
                //StorageFile storageFile = await StorageFile.GetFileFromPathAsync(file.FullName);
                //var properties = await storageFile.Properties.GetVideoPropertiesAsync();
                //Videos.Add(new VideoHelper
                //{
                //    FileName = file.Name,
                //    Duration = properties.Duration.ToString(@"hh\:mm\:ss"),
                //    DateModified = file.LastWriteTime,
                //    ThumbnailPath = await GenerateThumbnail(storageFile)
                //});
                //Debug.WriteLine(file.Name);
            }
        }

        public async Task LoadMP4Videos()
        {
            await LoadFilteredVideos(".mp4");
        }

        public async Task LoadMP3Videos()
        {
            await LoadFilteredVideos(".mp3");
        }

        private async Task LoadFilteredVideos(string extension)
        {
            await Task.Delay(500); // Wait 500ms before accessing files

            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "Prismarine");

            if (!Directory.Exists(folderPath)) return;

            var files = Directory.GetFiles(folderPath, $"*{extension}")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.LastWriteTime)
                .ToList();

            Videos.Clear();
            foreach (var file in files)
            {
                try
                {
                    StorageFile storageFile = await StorageFile.GetFileFromPathAsync(file.FullName);
                    var properties = extension == ".mp4"
                        ? (await storageFile.Properties.GetVideoPropertiesAsync()).Duration.ToString(@"hh\:mm\:ss")
                        : (await storageFile.Properties.GetMusicPropertiesAsync()).Duration.ToString(@"hh\:mm\:ss");

                    Videos.Add(new VideoHelper
                    {
                        FileName = file.Name,
                        Duration = properties,
                        DateModified = file.LastWriteTime,
                        ThumbnailPath = await GenerateThumbnail(storageFile)
                    });

                    Debug.WriteLine(file.Name);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private async Task<string> GenerateThumbnail(StorageFile file)
        {
            var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.VideosView, 100);
            if (thumbnail != null)
            {
                string thumbPath = Path.Combine(Path.GetTempPath(), file.Name + ".jpg");
                using var stream = thumbnail.AsStream();
                using var fileStream = new FileStream(thumbPath, FileMode.Create, FileAccess.Write);
                await stream.CopyToAsync(fileStream);
                return thumbPath;
            }
            return "default_thumbnail.png";
        }
    }
}
