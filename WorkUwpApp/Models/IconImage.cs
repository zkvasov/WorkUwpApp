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
            //GC.AddMemoryPressure(10 * 1024 * 1024);
            Name = file.Name;
            //SetPath();
            //SetPathAsync(file);
            File = file;
        }

        //~IconImage()
        //{
        //    GC.RemoveMemoryPressure(10 * 1024 * 1024);
        //}





        public async Task SetPathAsync(/*StorageFile file*/) //problem is here
        {

            //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Images/Geometry.jpg"));
            //using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            //{
            //    BitmapImage image = new BitmapImage();
            //    image.SetSource(fileStream);
            //    ImgSource = image;
            //}
            
            using (var randomAccessStream = await File.OpenAsync(FileAccessMode.Read))
            {
                 var bitmapImage = new BitmapImage();
                 //bitmapImage.UriSource = new Uri(file.Path);
                 bitmapImage.DecodePixelWidth = 100;
                 //StorageItemThumbnail imgThumbnail = await file.GetThumbnailAsync(
                 //    ThumbnailMode.PicturesView, 100, ThumbnailOptions.ResizeThumbnail);
                 //bitmapImage.SetSource(imgThumbnail);
                 bitmapImage.SetSource(randomAccessStream);
                 //bitmapImage.SetSourceAsync(randomAccessStream);
                 ImgSource = bitmapImage;
            }
        }
    }
}
