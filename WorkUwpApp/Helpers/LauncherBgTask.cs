using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace WorkUwpApp.Helpers
{
    public class LauncherBgTask
    {
        private const string taskName = "bgImage";
        private bool _isLaunched = false;

        public async void LaunhBgTask()
        {
            if (_isLaunched)
            {
                return;
            }
            var taskList = BackgroundTaskRegistration.AllTasks.Values;
            var task = taskList.FirstOrDefault(i => i.Name == taskName);
            if (task != null)
            {
                ((BackgroundTaskRegistration)task).Unregister(true);
            }

            
            var access = await BackgroundExecutionManager.RequestAccessAsync();
            //abort if access isn't granted
            if (access == BackgroundAccessStatus.DeniedBySystemPolicy)
            {
                return;
            }

            BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder
            {
                Name = taskName,
                TaskEntryPoint = typeof(RuntimeComponentForDesktop.DesktopBackgroundTask).ToString()
            };

            ApplicationTrigger appTrigger = new ApplicationTrigger();
            taskBuilder.SetTrigger(appTrigger);
            //if (task != null)
            //{
            //     ((BackgroundTaskRegistration)task).Unregister(true);
            //}

            //foreach(var task_ in BackgroundTaskRegistration.AllTasks.Values)
            //{
            //    if(task_.Name == taskName)
            //    {
            //        task_.Unregister(true);
            //    }
            //}

            task = taskBuilder.Register();
            task.Completed += new BackgroundTaskCompletedEventHandler(Task_Completed);
            await appTrigger.RequestAsync();

            foreach (var task_ in BackgroundTaskRegistration.AllTasks.Values)
            {
                if (task_.Name == taskName)
                {
                    _isLaunched = true;
                    break;
                }
            }
        }

        //public async void GetFolderWithImages()
        //{
        //    var folderPicker = new Windows.Storage.Pickers.FolderPicker
        //    {
        //        SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop
        //    };
        //    folderPicker.FileTypeFilter.Add(".jpg");
        //    folderPicker.FileTypeFilter.Add(".jpeg");
        //    folderPicker.FileTypeFilter.Add(".png");

        //    StorageFolder folder = await folderPicker.PickSingleFolderAsync();

        //    if (folder != null)
        //    {
        //        var storageItemAccessList = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(folder, folder.Name);
        //        ApplicationData.Current.LocalSettings.Values["storageItemAccessList"] = storageItemAccessList;
        //        //Application now has read/ write access to all contents in the picked folder
        //        // (including other sub - folder contents)
        //        Windows.Storage.AccessCache.StorageApplicationPermissions.
        //        FutureAccessList.AddOrReplace("PickedFolderToken", folder);

        //        LaunhBgTask();
        //    }

        //}

        private void Task_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            var taskList = BackgroundTaskRegistration.AllTasks.Values;
            var task = taskList.FirstOrDefault(i => i.Name == taskName);
            if (task != null)
            {
                task.Unregister(true);
            }

            Debug.WriteLine(message: "Background task completed at " + DateTime.Now.TimeOfDay);

            //ApplicationData.Current.LocalSettings.Values.Clear();
            //ApplicationData.Current.LocalSettings.Values.Values.Clear();
            //ApplicationData.Current.LocalSettings.DeleteContainer("path");
        }
    }
}

