using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WorkUwpApp
{
    class MainViewModel: ObservableObject
    {
        //private DesktopLoader desktopLoader_ = null;
        private string folderName_ = null;
        private LauncherBgTask launcher = null;

        public string FolderName
        {
            get => folderName_;
            set
            {
                folderName_ = value;
                OnPropertyChanged("FolderName");
            }
        }

        public ICommand ChooseBtnClicked
        {
            get
            {
                return new DelegateCommand(OpenFolder);
            }
        }

        public void OpenFolder()
        {
            //DesktopLoader.GetFolderWithImages();

            launcher = new LauncherBgTask();
            launcher.GetFolderWithImages();
            //launcher.LaunhBgTask();
        }

        //public ICommand RunBgTaskClicked
        //{
        //    get
        //    {
        //        return new DelegateCommand(RunBgTask);
        //    }
        //}

        //public void RunBgTask()
        //{
        //    if (launcher != null)
        //    {
        //        launcher.RunByAppTrigger();
        //    }
        //}
    }
}
