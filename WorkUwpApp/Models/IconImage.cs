using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WorkUwpApp.Helpers;

namespace WorkUwpApp.Models
{
    public class IconImage : ObservableObject
    {
        public string Name { get; private set; }
        public ImageSource ImgSource { get; private set; }
        public StorageFile File { get; private set;}

        public IconImage(StorageFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            Name = file.Name;
            File = file;
        }


        public async Task SetPathAsync() 
        {
            using (var randomAccessStream = await File.OpenAsync(FileAccessMode.Read))
            {
                 var bitmapImage = new BitmapImage();
                 bitmapImage.DecodePixelWidth = 100;
                 bitmapImage.SetSource(randomAccessStream);
                 ImgSource = bitmapImage;
            }
        }
    }
}
