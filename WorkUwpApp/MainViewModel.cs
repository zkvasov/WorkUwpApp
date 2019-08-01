using System.Windows.Input;

namespace WorkUwpApp
{
    class MainViewModel : ObservableObject
    {
        //Application app;
        //private DesktopLoader desktopLoader_ = null;
        private string folderName_ = null;
        private LauncherBgTask launcher = null;
       
        public string FolderName
        {
            get => folderName_;
            set
            {
                folderName_ = value;
                OnPropertyChanged(nameof(FolderName));
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
        }
    }
}
