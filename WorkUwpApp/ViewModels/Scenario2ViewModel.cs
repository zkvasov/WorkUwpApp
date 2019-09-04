using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;
using WorkUwpApp.Interfaces;
using WorkUwpApp.Models;
using WorkUwpApp.Helpers;

namespace WorkUwpApp.ViewModels
{
    public class Scenario2ViewModel : ViewModelBase, INavigable
    {
        private readonly INavigationService _navigationService;
        private string _nameCollection;
        private ImagesCollection _collection;
        private bool _isLoading = false;


        public ObservableCollection<IconImage> ImagesToAdd { get; private set; }
        public ObservableCollection<IconImage> SelectedImagesToAdd { get; private set; }
        public ObservableCollection<IconImage> CurrentImages { get; private set; }
        public ObservableCollection<IconImage> CurrentSelectedImages { get; private set; }

        public RelayCommand ChooseFolderClicked { get; private set; }
        public RelayCommand AddImagesClicked { get; private set; }
        public RelayCommand CancelClicked { get; private set; }
        public RelayCommand RemoveImagesClicked { get; private set; }
        public RelayCommand ReturnToListCommand { get; private set; }


        RelayCommand<List<IconImage>> _selectionChangedToAddCommand;
        public RelayCommand<List<IconImage>> SelectionChangedToAddCommand
           => _selectionChangedToAddCommand ?? (_selectionChangedToAddCommand = new RelayCommand<List<IconImage>>((l) =>
           {
               if (l != null)
                   SelectedImagesToAdd.Clear();
               foreach (var item in l)
               {
                   SelectedImagesToAdd.Add(item);
               }
           }, (l) => true));
        RelayCommand<List<IconImage>> _selectionChangedCurerentCommand;
        public RelayCommand<List<IconImage>> SelectionChangedCurrentCommand
           => _selectionChangedCurerentCommand ?? (_selectionChangedCurerentCommand = new RelayCommand<List<IconImage>>((l) =>
           {
               if (l != null)
                   CurrentSelectedImages.Clear();
               foreach (var item in l)
               {
                   CurrentSelectedImages.Add(item);
               }
           }, (l) => true));

        public Scenario2ViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            ImagesToAdd = new ObservableCollection<IconImage>();
            SelectedImagesToAdd = new ObservableCollection<IconImage>();
            CurrentImages = new ObservableCollection<IconImage>();
            CurrentSelectedImages = new ObservableCollection<IconImage>();

            ChooseFolderClicked = new RelayCommand(OpenFolder);
            AddImagesClicked = new RelayCommand(AddImages);
            CancelClicked = new RelayCommand(Cancel);
            RemoveImagesClicked = new RelayCommand(RemoveImages);
            ReturnToListCommand = new RelayCommand(ReturnCollection);

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
        public string NameCollection
        {
            get => _nameCollection;
            set => Set(ref _nameCollection, value);
        }
        public ImagesCollection Collection
        {
            get => _collection;
            set => Set(ref _collection, value);
        }

        public void OnNavigatedFrom(object sourceType)
        {
            //maybe in future
        }
        public void OnNavigatedTo(object parameter)
        {
            if (parameter is ImagesCollection)
            {
                Collection = (ImagesCollection)parameter;
                NameCollection = Collection.Name;
                GetImagesFromCollection(Collection);
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
                SelectedImagesToAdd.Clear();
                ImagesToAdd.Clear();
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
                    IconImage icon = new IconImage(image);
                    await icon.SetPathAsync().ConfigureAwait(true);
                    ImagesToAdd.Add(icon);
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
            foreach (IconImage image in SelectedImagesToAdd)
            {
                CurrentImages.Add(image);
            }
        }
        private void RemoveImages()
        {
           // IReadOnlyList<IconImage> imagesToRemove = CurrentSelectedImages;
            //foreach (IconImage image in CurrentSelectedImages)
            //{
            //    CurrentImages.Remove(image);
            //}
            while (CurrentSelectedImages.Count > 0)
            {
                var item = CurrentSelectedImages[CurrentSelectedImages.Count-1];
                CurrentImages.Remove(item);
            }
        }
        private void ReturnCollection()
        {
            if (Collection != null)
            {
                SetEditCollection();
                _navigationService.NavigateTo("Scenario3_CollectionsList", Collection);

            }
            ClearAll();
        }
        private void Cancel()
        {
            _navigationService.NavigateTo("Scenario3_CollectionsList");
            ClearAll();
        }
        private void ClearAll()
        {
            //TODO
       
            //
            SelectedImagesToAdd.Clear();
            ImagesToAdd.Clear();
            CurrentSelectedImages.Clear();
            CurrentImages.Clear();
            Collection = null;
        }

        private async void GetImagesFromCollection(ImagesCollection collection)
        {
            foreach(var imagePath in collection.ImagePaths)
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(imagePath);
                IconImage icon = new IconImage(file);
                await icon.SetPathAsync().ConfigureAwait(true);
                CurrentImages.Add(icon);
            }
        }
        private void SetEditCollection()
        {
            Collection.Name = NameCollection;
            Collection.ImagePaths.Clear();
            foreach(var image in CurrentImages)
            {
                Collection.ImagePaths.Add(image.File.Path);
            }
        }

        
    }
}
