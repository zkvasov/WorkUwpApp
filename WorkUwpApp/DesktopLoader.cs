using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public static async Task SetDesktopBackground(StorageFile file)
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                Debug.WriteLine("start setting BG");

                StorageFile localFile = await file.CopyAsync(ApplicationData.Current.LocalFolder, file.Name,
                                                                                         NameCollisionOption.ReplaceExisting);  /*сейчас тут вылетает*/
                Debug.WriteLine("after start");

                UserProfilePersonalizationSettings settings = UserProfilePersonalizationSettings.Current;
                Debug.WriteLine("between");

                bool isSuccess = await settings.TrySetWallpaperImageAsync(localFile);
                Debug.WriteLine("before BG");

                await Task.Delay(3000);
                //Thread.Sleep(3000);
                Debug.WriteLine("finish setting BG");
            }
        }

        public static async void GetFolderWithImages()
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add(".jpg");
            folderPicker.FileTypeFilter.Add(".jpeg");
            folderPicker.FileTypeFilter.Add(".png");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            //StorageFolder folder2 = await StorageFolder.GetFolderFromPathAsync(folder.Path);
            var fileList = await folder.GetFilesAsync();
            if (folder != null)
            {
                // ApplicationData.Current.LocalSettings.Values["path"] = folder.Path;

                //IReadOnlyList<StorageFile> fileList = await folder.GetFilesAsync();

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
