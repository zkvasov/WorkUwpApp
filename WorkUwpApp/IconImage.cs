using System;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace WorkUwpApp
{
    class IconImage : ObservableObject
    {
        public string Name { get; private set; }
        public ImageSource ImgSource { get; private set; }
        public StorageFile File { get; private set;}

        public IconImage(StorageFile file)
        {
            Name = file.Name;
            //SetPath();
            SetPath(file);
            File = file;
        }


        private async void SetPath(StorageFile file)
        {

            //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Images/Geometry.jpg"));
            //using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            //{
            //    BitmapImage image = new BitmapImage();
            //    image.SetSource(fileStream);
            //    ImgSource = image;
            //}

            using (var randomAccessStream = await file.OpenAsync(FileAccessMode.Read))
            {
                var bitmapImage = new BitmapImage();
                //bitmapImage.UriSource = new Uri(file.Path);
                bitmapImage.DecodePixelWidth = 100;
                //StorageItemThumbnail imgThumbnail = await file.GetThumbnailAsync(
                //    ThumbnailMode.PicturesView, 100, ThumbnailOptions.ResizeThumbnail);
                //bitmapImage.SetSource(imgThumbnail);
                bitmapImage.SetSource(randomAccessStream);
                //await bitmapImage.SetSourceAsync(randomAccessStream);
                ImgSource = bitmapImage;
            }
        }
    }
}
