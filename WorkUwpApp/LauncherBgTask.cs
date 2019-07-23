using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Core;

namespace WorkUwpApp
{
    public class LauncherBgTask
    {
        private const string taskName = "bgImage";

        public async void LaunhBgTask()
        {
            //ApplicationData.Current.LocalSettings.Values["number"] = 6; // число для подсчета факториала
            var taskList = BackgroundTaskRegistration.AllTasks.Values;
            var task = taskList.FirstOrDefault(i => i.Name == taskName);
            if (task == null)
            {
                var taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = typeof(RuntimeComponentForDesktop.DesktopBackgroundTask).ToString();

                ApplicationTrigger appTrigger = new ApplicationTrigger();
                taskBuilder.SetTrigger(appTrigger);

                task = taskBuilder.Register();

               // task.Progress += new BackgroundTaskProgressEventHandler(Task_Progress);
                //task.Completed += new BackgroundTaskCompletedEventHandler(Task_Completed);

                await appTrigger.RequestAsync();
                
            }
        }

        //private void Stop_Click(object sender, RoutedEventArgs e)
        //{
        //    Stop();
        //}

        //private void Task_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        //{
        //    var result = ApplicationData.Current.LocalSettings.Values["factorial"];
        //    var progress = $"Результат: {result}";
        //  //  UpdateUI(progress);
        //    Stop();
        //}

        //private void Task_Progress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
        //{
        //    var progress = $"Progress: {args.Progress} %";
        //   // UpdateUI(progress);
        //}

        //private async void UpdateUI(string progress)
        //{
        //    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
        //    () =>
        //    {
        //        outputBlock.Text = progress;
        //    });
        //}

        //private async void Stop()
        //{
        //    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
        //    () =>
        //    {
        //        var taskList = BackgroundTaskRegistration.AllTasks.Values;
        //        var task = taskList.FirstOrDefault(i => i.Name == taskName);
        //        if (task != null)
        //        {
        //            task.Unregister(true);

        //            stopButton.IsEnabled = false;
        //            startButton.IsEnabled = true;
        //        }
        //    });
        //}
    }
}

