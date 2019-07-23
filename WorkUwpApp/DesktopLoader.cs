using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.UserProfile;

namespace WorkUwpApp
{
    public class DesktopLoader/* : ObservableObject*/
    {
       // public ObservableCollection<string> selectedImages = new ObservableCollection<string>();

        //public string FolderName { get; private set; }

        //private string _folderName;

        //public string FolderName
        //{
        //    get => _folderName;
        //    set
        //    {
        //        _folderName = value;
        //        OnPropertyChanged("FolderName");
        //    }
        //}

        public async Task SetDesktopBackground(StorageFile file)
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Geometry.jpg"));
                // файл из приложения не может быть установлен в качестве заставки, поэтому копируем его в локальную папку
                StorageFile localFile = await file.CopyAsync(ApplicationData.Current.LocalFolder, file.Name/*"Geometry.jpg"*/,
                                                                                         NameCollisionOption.ReplaceExisting);

                UserProfilePersonalizationSettings settings = UserProfilePersonalizationSettings.Current;
                bool isSuccess = await settings.TrySetWallpaperImageAsync(localFile);
                await Task.Delay(3000);
                //Thread.Sleep(3000);
            }
        }

        public async void OpenFolder()
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add(".jpg");
            folderPicker.FileTypeFilter.Add(".jpeg");
            folderPicker.FileTypeFilter.Add(".png");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            
            if (folder != null)
            {
                IReadOnlyList<StorageFile> fileList = await folder.GetFilesAsync();
                
                foreach (StorageFile file in fileList)
                {
                    await SetDesktopBackground(file);
                }

                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                //Windows.Storage.AccessCache.StorageApplicationPermissions.
                //FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                //FolderName = folder.Name;
            }
            
        }

    }
}
