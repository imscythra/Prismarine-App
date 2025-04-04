using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Prismarine.Helpers
{
    class DownloadHelper
    {
        private readonly YoutubeClient _youtube; // YouTube client

        public DownloadHelper()
        {
            _youtube = new YoutubeClient(); // Initialize client
        }

        public async Task<StreamManifest?> GetAvailableStreamsAsync(string videoUrl)
        {
            try
            {
                return await _youtube.Videos.Streams.GetManifestAsync(videoUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching streams: {ex.Message}");
                return null;
            }
        }
    }
}
