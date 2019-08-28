//using System.Collections.ObjectModel;
//using System.Windows.Input;
//using Windows.Storage;
//using System;
//using System.Diagnostics;
//using System.Threading.Tasks;
//using WorkUwpApp.Models;
//using WorkUwpApp.ViewModels.Helpers;
//using System.Collections;
//using Windows.UI.Xaml.Controls;

//namespace WorkUwpApp.ViewModels
//{
//    internal class MainViewModel : ObservableObject
//    {
//        private const string _containerName = "imagesContainer";
//        private const string _imageSetting = "image";
//        private LauncherBgTask launcher;
//        private string _nameNewCollection = "New collection";
//        private IconImage _selectedImage;
//        private ImagesCollection _selectedCollection;
//        private int _selectedInterval = 5;

//        public MainViewModel()
//        {
//            launcher = new LauncherBgTask();
//        }

//        public string NameNewCollection
//        {
//            get => _nameNewCollection;
//            set => Set(ref _nameNewCollection, value);
//        }
//        public IconImage SelectedImage
//        {
//            get => _selectedImage;
//            set
//            {
//                Set(ref _selectedImage, value);
//                OnPropertyChanged(nameof(IsSelectedImage));
//            }
//        }
//        public bool IsSelectedImage => SelectedImage != null;
//        public ImagesCollection SelectedCollection
//        {
//            get => _selectedCollection;
//            set
//            {
//                Set(ref _selectedCollection, value);
//                OnPropertyChanged(nameof(IsSelectedCollection));
//            }
//        }
//        public bool IsSelectedCollection => SelectedCollection != null;
//        public int SelectedInterval
//        {
//            get => _selectedInterval;
//            set => Set(ref _selectedInterval, value);
//        }

//        //public ObservableCollection<ImagesCollection> Collections { get; } = new ObservableCollection<ImagesCollection>();
//        //public ObservableCollection<IconImage> Icons { get; } = new ObservableCollection<IconImage>();
//        //public ObservableCollection<object> SelectedImages { get; } = new ObservableCollection<object>();

//        //public System.Collections.IList SelectedItems    
//        //{
//        //    get
//        //    {
//        //        return SelectedImages;
//        //    }
//        //    set
//        //    {
//        //        SelectedImages.Clear();
//        //        foreach (IconImage image in value)             //NullReferenceException
//        //        {
//        //            SelectedImages.Add(image);
//        //        }
//        //    }
//        //}

//        public ICommand ChooseBtnClicked => new DelegateCommand(OpenFolder);
//        public ICommand AddNewCollectionClicked => new DelegateCommand(AddNewCollection);
//        public ICommand RemoveCollectionClicked => new DelegateCommand(RemoveCollection);
//        public ICommand AddImagesClicked => new DelegateCommand(AddImages);
//        public ICommand PlayInBgClicked => new DelegateCommand(PlayInBg);
//        //public ICommand SetSelectedImagesChanged => new ComplexCommand(new Action<object>(SetSelectedImages));
//        public ICommand ReturnToListClicked => new DelegateCommand(ReturnToList);

//        private async void OpenFolder()
//        {
//            var folderPicker = new Windows.Storage.Pickers.FolderPicker
//            {
//                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop
//            };
//            folderPicker.FileTypeFilter.Add(".jpg");
//            folderPicker.FileTypeFilter.Add(".jpeg");
//            folderPicker.FileTypeFilter.Add(".png");

//            StorageFolder folder = await folderPicker.PickSingleFolderAsync();

//            if (folder != null)
//            {
//                SelectedImages.Clear();
//                Icons.Clear();
//                await GetImagesFromFolder(folder).ConfigureAwait(false);
//            }
//        }
//        private void AddNewCollection()
//        {
//            //StorageFolder newFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(
//            //    NameNewCollection, CreationCollisionOption.GenerateUniqueName);
//            ImagesCollection collection = new ImagesCollection(NameNewCollection);
//            Collections.Add(collection);
//        }
//        private void RemoveCollection()
//        {
//            //await SelectedCollection.StorageFolder.DeleteAsync();
//            Collections.Remove(SelectedCollection);
//        }
//        private async Task GetImagesFromFolder(StorageFolder folder)
//        {
//            var images = await folder.GetFilesAsync();
//            foreach (var image in images)
//            {
//                if ((image.FileType == ".jpg") ||
//                    (image.FileType == ".JPG") ||
//                    (image.FileType == ".jpeg") ||
//                    (image.FileType == ".png"))
//                {
//                    Icons.Add(new IconImage(image));
//                }
//            }

//            //var items = await folder.GetItemsAsync();
//            //foreach (var item in items)
//            //{
//            //    if (item is StorageFile)
//            //    {
//            //        Icons.Add(new IconImage((StorageFile)item));
//            //    }
//            //    else
//            //    {
//            //        await GetImagesFromFolder((StorageFolder)item).ConfigureAwait(false);
//            //    }
//            //}
//        }
//        private void AddImages()
//        {
//            foreach (IconImage image in SelectedImages)
//            {
//                SelectedCollection.AddImage(image.File);
//            }
//        }
//        private void PlayInBg()
//        {
//            //ApplicationData.Current.LocalSettings.Values["interval"] = _selectedInterval;
//            //var storageItemAccessList = Windows.Storage.AccessCache.StorageApplicationPermissions.
//            //    FutureAccessList.Add(SelectedCollection.StorageFolder, SelectedCollection.StorageFolder.Name);
//            //ApplicationData.Current.LocalSettings.Values["storageItemAccessList"] = storageItemAccessList;
//            ////Application now has read/ write access to all contents in the picked folder
//            //// (including other sub - folder contents)
//            //Windows.Storage.AccessCache.StorageApplicationPermissions.
//            //FutureAccessList.AddOrReplace("PickedFolderToken", SelectedCollection.StorageFolder);

//            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
//            localSettings.CreateContainer(
//                _containerName, ApplicationDataCreateDisposition.Always);
//            if (localSettings.Containers.ContainsKey(_containerName))
//            {
//                int count = 0;
//                foreach (var file in SelectedCollection.Images)
//                {
//                    string fileToken = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);

//                    localSettings.Containers[_containerName].Values[String.Format(_imageSetting + count.ToString())] = fileToken;
//                    count++;
//                }
//            }

//            launcher.LaunhBgTask();
//        }

//        //private void SetSelectedImages(object list)
//        //{
//        //    SelectedImages.Clear();
//        //    var images = (ObservableCollection<IconImage>)list;
//        //    foreach (IconImage image in images)
//        //    {
//        //        SelectedImages.Add(image);
//        //    }
//        //}

//        private void ReturnToList()                //to do
//        {
//            //Frame.Navigate(typeof(Scenario3_CollectionsList));
//        }
//    }
//}
