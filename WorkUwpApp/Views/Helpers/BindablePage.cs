using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WorkUwpApp.Interfaces;

namespace WorkUwpApp.Views.Helpers
{
    public class BindablePage : Page
    {
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            var navigableViewModel = this.DataContext as INavigable;
            if (navigableViewModel != null)
            {
                //navigableViewModel.OnNavigatedTo(e.Parameter);

                navigableViewModel.OnNavigatedFrom(e.SourcePageType.Name);
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            var navigableViewModel = this.DataContext as INavigable;
            if (navigableViewModel != null)
            {
                //navigableViewModel.OnNavigatedTo(e.Parameter);
                
                navigableViewModel.OnNavigatedTo(e.Parameter);
            }
        }
        //protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        //{
        //    base.OnNavigatingFrom(e);
        //    if (e.NavigationMode == NavigationMode.Back)
        //    {
        //        ResetPageCache();
        //    }
        //}

        //private void ResetPageCache()
        //{
        //    var frame = Window.Current.Content as Frame;
        //    frame.Content = null;
        //    //var cacheSize = ((Frame)Parent).CacheSize;
        //    //((Frame)Parent).CacheSize = 0;
        //    //((Frame)Parent).CacheSize = cacheSize;
        //}
        
    }
}
