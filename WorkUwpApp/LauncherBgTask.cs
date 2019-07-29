using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace WorkUwpApp
{
    public class LauncherBgTask
    {
        private const string taskName = "bgImage";

        private BackgroundTaskBuilder taskBuilder = null;
        private ApplicationTrigger appTrigger = null;

        public async void LaunhBgTask()
        {
            var taskList = BackgroundTaskRegistration.AllTasks.Values;
            var task = taskList.FirstOrDefault(i => i.Name == taskName);
            if (task != null)
            {
                task.Unregister(true);
            }
            
            //DesktopLoader.GetFolderWithImages();                         // set path to folder in ApplicationData.Current.LocalSettings
            taskBuilder = new BackgroundTaskBuilder();
            taskBuilder.Name = taskName;
            taskBuilder.TaskEntryPoint = typeof(RuntimeComponentForDesktop.DesktopBackgroundTask).ToString();

            appTrigger = new ApplicationTrigger();
            taskBuilder.SetTrigger(appTrigger);

            var access = await BackgroundExecutionManager.RequestAccessAsync();

            //abort if access isn't granted
            if (access == BackgroundAccessStatus.DeniedBySystemPolicy)
            {
                return;
            }

            task = taskBuilder.Register();

            // task.Progress += new BackgroundTaskProgressEventHandler(Task_Progress);
            task.Completed += new BackgroundTaskCompletedEventHandler(Task_Completed);

            await appTrigger.RequestAsync();
        }

        //public async void RunByAppTrigger()
        //{
        //    await appTrigger.RequestAsync();
        //}

        public async void GetFolderWithImages()
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add(".jpg");
            folderPicker.FileTypeFilter.Add(".jpeg");
            folderPicker.FileTypeFilter.Add(".png");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                //var storageItemAccessList = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList;
                //storageItemAccessList.Add(folder);
                var storageItemAccessList = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(folder, folder.Name);
                ////ApplicationData.Current.LocalSettings.Values["path"] = folder.Path;
                ApplicationData.Current.LocalSettings.Values["storageItemAccessList"] = storageItemAccessList;
                //Application now has read/ write access to all contents in the picked folder
                // (including other sub - folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", folder);

                LaunhBgTask();

                //IReadOnlyList<StorageFile> fileList = await folder.GetFilesAsync();

                //foreach (StorageFile file in fileList)
                //{
                //    await SetDesktopBackground(file);
                //}

                
            }

        }

        private void Task_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            var taskList = BackgroundTaskRegistration.AllTasks.Values;
            var task = taskList.FirstOrDefault(i => i.Name == taskName);
            if (task != null)
            {
                task.Unregister(true);
            }

            Debug.WriteLine(string.Format("Background task completed at {0}", DateTime.Now.TimeOfDay));

            // ApplicationData.Current.LocalSettings.Values.Clear();
            //ApplicationData.Current.LocalSettings.Values.Values.Clear();
            //ApplicationData.Current.LocalSettings.DeleteContainer("path");
        }
    }
}

