using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using WorkUwpApp.Models;

namespace WorkUwpApp.ViewModels
{
    public class Scenario1ViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private ImagesCollection _collection;
        private bool _isLoading = false;
        private bool _isCollectionCreated = false;
        private string _nameNewCollection = "New collection";


        public ObservableCollection<IconImage> Icons { get; private set; }
        public ObservableCollection<object> SelectedImages { get; private set; }

        public RelayCommand ChooseFolderClicked { get; private set; }
        public RelayCommand AddImagesClicked { get; private set; }
        public RelayCommand CreateNewCollectionClicked { get; private set; }
        public RelayCommand AddCollectionClicked { get; private set; }
        public RelayCommand CancelClicked { get; private set; }



        public Scenario1ViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Icons = new ObservableCollection<IconImage>();
            SelectedImages = new ObservableCollection<object>();
            ChooseFolderClicked = new RelayCommand(OpenFolder);
            AddImagesClicked = new RelayCommand(AddImages);
            CreateNewCollectionClicked = new RelayCommand(CreateNewCollection);
            AddCollectionClicked = new RelayCommand(AddCollectionToList);
            CancelClicked = new RelayCommand(Cancel);
        }

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                RaisePropertyChanged(nameof(IsLoading));
            }
        }
        public string NameNewCollection
        {
            get => _nameNewCollection;
            set => Set(ref _nameNewCollection, value);
        }
        public ImagesCollection Collection
        {
            get => _collection;
            set
            {
                Set(ref _collection, value);
                RaisePropertyChanged(nameof(IsCollectionCreated));
            }
        }
        public bool IsCollectionCreated
        {
            get
            {
                return _isCollectionCreated;
            }
            set
            {
                _isCollectionCreated = value;
                RaisePropertyChanged(nameof(IsCollectionCreated));
            }
        }
        private async void OpenFolder()
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop
            };
            folderPicker.FileTypeFilter.Add(".jpg");
            folderPicker.FileTypeFilter.Add(".JPG");
            folderPicker.FileTypeFilter.Add(".jpeg");
            folderPicker.FileTypeFilter.Add(".png");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                SelectedImages.Clear();
                Icons.Clear();
                await GetImagesFromFolder(folder).ConfigureAwait(false);
            }
        }
        private async Task GetImagesFromFolder(StorageFolder folder)
        {
            var images = await folder.GetFilesAsync();
            foreach (var image in images)
            {
                if ((image.FileType == ".jpg") ||
                    (image.FileType == ".JPG") ||
                    (image.FileType == ".jpeg") ||
                    (image.FileType == ".png"))
                {
                    Icons.Add(new IconImage(image));
                }
            }

            //var items = await folder.GetItemsAsync();
            //foreach (var item in items)
            //{
            //    if (item is StorageFile)
            //    {
            //        Icons.Add(new IconImage((StorageFile)item));
            //    }
            //    else
            //    {
            //        await GetImagesFromFolder((StorageFolder)item).ConfigureAwait(false);
            //    }
            //}
        }
        private void AddImages()
        {
            foreach (IconImage image in SelectedImages)
            {
                Collection.AddImage(image.File);
            }
        }
        private void CreateNewCollection()
        {
            Collection = new ImagesCollection(NameNewCollection);
        }
        private void AddCollectionToList()
        {
            //Cleanup();

            _navigationService.NavigateTo("Scenario3_CollectionsList", Collection);
           //ClearAll();
        }
        private void Cancel()
        {
            //Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);

            Cleanup();
            _navigationService.GoBack();

            //ClearAll();
        }
        private void ClearAll()
        {
            _nameNewCollection = "New collection";
            _isCollectionCreated = false;
            SelectedImages.Clear();
            Icons.Clear();
            Collection = null;
        }

        public override void Cleanup()
        {
            Messenger.Default.Unregister<Scenario1ViewModel>(this);
            base.Cleanup();
        }
    }
}
