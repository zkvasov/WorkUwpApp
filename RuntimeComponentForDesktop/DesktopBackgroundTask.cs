using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.System.UserProfile;

namespace RuntimeComponentForDesktop
{
    public sealed class DesktopBackgroundTask : IBackgroundTask
    {
        CancellationTokenSource cancel = new CancellationTokenSource();
        volatile bool _cancelRequested = false; // прервана ли задача

        private List<StorageFile> _imageFiles;
        
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine(taskInstance.Task.Name);
            //оценка стоимости выполнения задачи для приложения
            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            if (cost == BackgroundWorkCostValue.High)
                return;

            // обрабатываем прерывание задачи
            taskInstance.Canceled += (s, e) =>
            {
                cancel.Cancel();
                cancel.Dispose();
                _cancelRequested = true;
            };

            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();
            await LoadBgImage();
            //SetFromResources();
            _deferral.Complete();
        }

        private async Task GetFilesInFolder(StorageFolder folder)
        {
            var items = await folder.GetItemsAsync();
            foreach (var item in items)
            {
                if (item is StorageFile)
                {
                    _imageFiles.Add((StorageFile)item);
                }
                else
                {
                    await GetFilesInFolder((StorageFolder)item);
                }
            }
        }

        private async Task LoadBgImage()
        {
            // получаем локальные настройки приложения
            var settings = ApplicationData.Current.LocalSettings;
            string accsessFolder = (string)settings.Values["storageItemAccessList"];

            if (accsessFolder != null)
            {
                StorageFolder folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(accsessFolder);
                if (folder != null)
                {
                    _imageFiles = new List<StorageFile>();
                    await GetFilesInFolder(folder);
                    try
                    {
                        FolderHandling(_imageFiles);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }

                    //IReadOnlyList<StorageFile> imageFiles = null;
                    //////////////////////////////////
                    //try
                    //{
                    //    imageFiles = await folder.GetFilesAsync();
                    //    FolderHandling(imageFiles);
                    //}
                    //catch (Exception e)
                    //{

                    //}
                }
            }
            settings.Values.Remove("storageItemAccessList");
        }

        private void FolderHandling(IReadOnlyList<StorageFile> imageFiles)
        {

            //бесконечно в кольцевом порядке
            int i = 0;
            while (!_cancelRequested)       //если задача прервана, выходим из цикла
            {
                if (i >= imageFiles.Count)
                {
                    i = 0;
                }

                SetDesktopBackground(imageFiles[i]);
                //await Task.Delay(3000);
                Thread.Sleep(3000);                    //интервал между загрузкой изображений
                i++;
            }
        }

        private void SetDesktopBackground(StorageFile file)
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {

                var outer = Task.Factory.StartNew(async() =>      // внешняя задача
                {
                    StorageFile localFile = await file.CopyAsync(ApplicationData.Current.LocalFolder, file.Name,
                                                                                             NameCollisionOption.ReplaceExisting); 
                    var inner = Task.Factory.StartNew(async () =>  // вложенная задача
                    {
                        UserProfilePersonalizationSettings settings = UserProfilePersonalizationSettings.Current;
                        bool isSuccess = await settings.TrySetWallpaperImageAsync(localFile);
                    }, TaskCreationOptions.AttachedToParent);
                });
                outer.Wait(); // ожидаем выполнения внешней задачи
            }
        }
        
        //private async void SetFromResources()
        //{
        //    StorageFolder imagesFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Images");
        //    if (imagesFolder != null)
        //    {
        //        IReadOnlyList<StorageFile> imageFiles = null;
        //        ////////////////////////////////
        //        try
        //        {
        //            imageFiles = await imagesFolder.GetFilesAsync();
        //            FolderHandling(imageFiles);
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.WriteLine(e.ToString());
        //        }
        //    }
        //}
    }
}
