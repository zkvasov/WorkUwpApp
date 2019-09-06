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

        private const string _containerName = "imagesContainer";
        private const string _imageSetting = "image";
        private const string _intervalSetting = "interval";
        private const string _containerChangedSetting = "containerChangedSetting";
        private const string _isLaunchedKey = "IsLaunchedBg";


        readonly CancellationTokenSource cancel = new CancellationTokenSource();
        volatile bool _cancelRequested = false;   // прервана ли задача
         
        private List<StorageFile> _imageFiles;
        private int _interval;    //интервал между загрузкой изображений

        public void Run(IBackgroundTaskInstance taskInstance)
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
                //ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                //localSettings.Values[_isLaunchedKey] = false;
            };

            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();
            GetDataFromSettings();
            //await LoadBgImage();
            //SetFromResources();
            _deferral.Complete();
        }

        private async void GetDataFromSettings()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            int seconds = (int)localSettings.Values[_intervalSetting];
            if (seconds >= 1)
            {
                _interval = seconds * 1000;
            }

            bool hasContainer = localSettings.Containers.ContainsKey(_containerName);
            if (hasContainer)
            {
                _imageFiles = new List<StorageFile>();
                int count = 0;
                foreach(var token in localSettings.Containers[_containerName].Values)
                {
                    string fileKey = _imageSetting + count.ToString();
                    if (localSettings.Containers[_containerName].Values.ContainsKey(fileKey))
                    {
                        string tokenFile = (string)localSettings.Containers[_containerName].Values[fileKey];
                        StorageFile file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(tokenFile);
                        if (file != null)
                        {
                            _imageFiles.Add(file);
                        }
                    }
                    count++;
                }

                localSettings.Values[_containerChangedSetting] = false;
                FolderHandling(_imageFiles);
            }
        }

        //private async Task GetFilesInFolder(StorageFolder folder)
        //{
        //    var items = await folder.GetItemsAsync();
        //    foreach (var item in items)
        //    {
        //        if (item is StorageFile)
        //        {
        //            _imageFiles.Add((StorageFile)item);
        //        }
        //        else
        //        {
        //            await GetFilesInFolder((StorageFolder)item);
        //        }
        //    }
        //}


        //private async Task LoadBgImage()
        //{
        //    // получаем локальные настройки приложения
        //    var settings = ApplicationData.Current.LocalSettings;
        //    int seconds = (int)settings.Values["interval"];
        //    if (seconds >= 3 && seconds <= 30)
        //    {
        //        _interval = seconds * 1000;
        //    }
        //    string accsessFolder = (string)settings.Values["storageItemAccessList"];

        //    if (accsessFolder != null)
        //    {
        //        StorageFolder folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(accsessFolder);
        //        if (folder != null)
        //        {
        //            _imageFiles = new List<StorageFile>();
        //            await GetFilesInFolder(folder);
        //            try
        //            {
        //                FolderHandling(_imageFiles);
        //            }
        //            catch (Exception e)
        //            {
        //                Debug.WriteLine(e.ToString());
        //            }
        //        }
        //    }
        //   //settings.Values.Remove("storageItemAccessList");
        //   //settings.Values.Remove("interval");
        //}

        private void FolderHandling(IReadOnlyList<StorageFile> imageFiles)
        {
            //бесконечно  как в кольцевом списке
            int i = 0;
            while (!_cancelRequested)       //если задача прервана, выходим из цикла
            {
                if (i >= imageFiles.Count)
                {
                    i = 0;
                }

                SetDesktopBackground(imageFiles[i]);
                //await Task.Delay(_interval);
                Thread.Sleep(_interval);                    //интервал между загрузкой изображений
                i++;
                var localSettings = ApplicationData.Current.LocalSettings;
                bool isCollectionChanged = (bool)localSettings.Values[_containerChangedSetting];
                if (isCollectionChanged)
                {
                    GetDataFromSettings();
                }
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
