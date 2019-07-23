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
        private DesktopLoader desktopLoader_ = null;
        private string folderName_ = null;

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
            desktopLoader_ = new DesktopLoader();
            desktopLoader_.OpenFolder();
            //FolderName = desktopLoader_.FolderName;
        }
    }
}
