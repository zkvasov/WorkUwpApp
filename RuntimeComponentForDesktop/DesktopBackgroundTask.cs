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
    public sealed class DesktopBackgroundTask : IBackgroundTask, IDisposable
    {

        private const string _containerName = "imagesContainer";
        private const string _imageSetting = "image";
        private const string _intervalSetting = "interval";
        private const string _containerChangedSetting = "containerChangedSetting";
        private const string _isLaunchedKey = "IsLaunchedBg";


        private readonly CancellationTokenSource cancel = new CancellationTokenSource();
        volatile bool _cancelRequested = false;   // прервана ли задача
         
        private List<StorageFile> _imageFiles;
        private int _interval;    //интервал между загрузкой изображений

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            if (taskInstance == null)
            {
                throw new ArgumentNullException(nameof(taskInstance));
            }
            Debug.WriteLine(taskInstance.Task.Name);
            //оценка стоимости выполнения задачи для приложения
            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            if (cost == BackgroundWorkCostValue.High)
                return;

            // TODO: обрабатываем прерывание задачи
            taskInstance.Canceled += (s, e) =>
            {
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values[_isLaunchedKey] = false;
                cancel.Cancel();
                cancel.Dispose();
                _cancelRequested = true;

            };

            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();

            await GetDataFromSettings().ConfigureAwait(true);

            _deferral.Complete();
        }

        private async Task GetDataFromSettings()
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
                await FolderHandling(_imageFiles).ConfigureAwait(true);
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


        private async Task FolderHandling(IReadOnlyList<StorageFile> imageFiles)
        {
            //бесконечно  как в кольцевом списке
            int i = 0;
            while (!_cancelRequested)       //если задача прервана, выходим из цикла
            {
                if (i >= imageFiles.Count)
                {
                    i = 0;
                }

                await SetDesktopBackgroundAsync(imageFiles[i]).ConfigureAwait(false);
                Thread.Sleep(_interval);                    //интервал между загрузкой изображений
                i++;
                var localSettings = ApplicationData.Current.LocalSettings;
                bool isCollectionChanged = (bool)localSettings.Values[_containerChangedSetting];
                if (isCollectionChanged)
                {
                    await GetDataFromSettings().ConfigureAwait(false);
                }
            }
        }
        private static async Task SetDesktopBackgroundAsync(StorageFile file)
        {
           
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                StorageFile localFile = await file.CopyAsync(ApplicationData.Current.LocalFolder, file.Name,
                                                                                       NameCollisionOption.ReplaceExisting);
                UserProfilePersonalizationSettings settings = UserProfilePersonalizationSettings.Current;
                await settings.TrySetWallpaperImageAsync(localFile);
            }
        }

        public void Dispose()
        {
            cancel.Dispose();
        }
    }
}
