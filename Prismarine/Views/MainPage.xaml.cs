using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Prismarine.Helpers;
using System.Diagnostics;
using Windows.Storage;
using Windows.System;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Prismarine.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public VideoManager ViewModel { get; } = new(); // Create an instance
        public MainPage()
        {
            this.InitializeComponent();
            LoadHelper(); // Load videos on startup
        }

        private async void debugRefreshList_Click(object sender, RoutedEventArgs e)
        {
            LoadHelper();
        }

        private async void fileList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is VideoHelper selectedVideo)
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "Prismarine", selectedVideo.FileName);

                if (File.Exists(filePath))
                {
                    await Launcher.LaunchFileAsync(await StorageFile.GetFileFromPathAsync(filePath));
                }
                else
                {
                    Debug.WriteLine("File not found: " + filePath);
                }
            }
        }



        private void filterOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadHelper();
        }

        private async void LoadHelper()
        {
            await Task.Delay(20);
            debugRefreshList.IsEnabled = false;
            refreshPresenter.Value = "Working";
            filterOption.IsEnabled = false;
            if (filterOption.SelectedIndex == 0) { await ViewModel.LoadVideos(); }
            else if (filterOption.SelectedIndex == 1) { await ViewModel.LoadMP4Videos(); }
            else if (filterOption.SelectedIndex == 2) { await ViewModel.LoadMP3Videos(); }
            debugRefreshList.IsEnabled = true;
            refreshPresenter.Value = "Idle";
            filterOption.IsEnabled = true;
        }
    }
}
