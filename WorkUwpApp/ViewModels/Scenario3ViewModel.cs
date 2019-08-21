using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Windows.Storage;
using WorkUwpApp.Models;
using WorkUwpApp.ViewModels.Helpers;
using WorkUwpApp.Interfaces;

namespace WorkUwpApp.ViewModels
{
    public class Scenario3ViewModel : ViewModelBase, INavigable
    {
        private readonly INavigationService _navigationService;
        private const string _containerName = "imagesContainer";
        private const string _imageSetting = "image";
        private LauncherBgTask _launcher;
        private ImagesCollection _selectedCollection;
        private int _selectedInterval = 5;
        private bool _isLoading = false;
        private string _title;
        public ObservableCollection<ImagesCollection> Collections { get; }

        public RelayCommand AddNewCollectionClicked { get; private set; }
        public RelayCommand EditCollectionClicked { get; private set; }
        public RelayCommand RemoveCollectionClicked { get; private set; }
        public RelayCommand PlayInBgClicked { get; private set; }

        

        public Scenario3ViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _launcher = new LauncherBgTask();
            Collections = new ObservableCollection<ImagesCollection>();
            Title = "Image collections";
            AddNewCollectionClicked = new RelayCommand(AddNewCollection);
            EditCollectionClicked = new RelayCommand(EditCollection);
            RemoveCollectionClicked = new RelayCommand(RemoveCollection);
            PlayInBgClicked = new RelayCommand(PlayInBg);
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
        public string Title //unnecessary
        {

            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    RaisePropertyChanged(nameof(Title));
                }
            }
        }
        public ImagesCollection SelectedCollection
        {
            get => _selectedCollection;
            set
            {
                Set(ref _selectedCollection, value);
                RaisePropertyChanged(nameof(IsSelectedCollection));
            }
        }
        public bool IsSelectedCollection => SelectedCollection != null;
        public int SelectedInterval
        {
            get => _selectedInterval;
            set => Set(ref _selectedInterval, value);
        }

        private void AddNewCollection()
        {
            _navigationService.NavigateTo("Scenario1_CreateCollection");
        }
        private void EditCollection()
        {
            if (IsSelectedCollection)
            {
                _navigationService.NavigateTo("Scenario2_CollectionEditor", SelectedCollection);
            }
        }
        private void RemoveCollection()
        {
            Collections.Remove(SelectedCollection);
        }
        private void PlayInBg()
        {
            //ApplicationData.Current.LocalSettings.Values["interval"] = _selectedInterval;
            //var storageItemAccessList = Windows.Storage.AccessCache.StorageApplicationPermissions.
            //    FutureAccessList.Add(SelectedCollection.StorageFolder, SelectedCollection.StorageFolder.Name);
            //ApplicationData.Current.LocalSettings.Values["storageItemAccessList"] = storageItemAccessList;
            ////Application now has read/ write access to all contents in the picked folder
            //// (including other sub - folder contents)
            //Windows.Storage.AccessCache.StorageApplicationPermissions.
            //FutureAccessList.AddOrReplace("PickedFolderToken", SelectedCollection.StorageFolder);

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.CreateContainer(
                _containerName, ApplicationDataCreateDisposition.Always);
            if (localSettings.Containers.ContainsKey(_containerName))
            {
                int count = 0;
                foreach (var file in SelectedCollection.Images)
                {
                    string fileToken = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);

                    localSettings.Containers[_containerName].Values[String.Format(_imageSetting + count.ToString())] = fileToken;
                    count++;
                }
            }

            _launcher.LaunhBgTask();
        }

        public void OnNavigatedTo(object parameter)
        {
            if (parameter is ImagesCollection)
            {
                Collections.Add((ImagesCollection)parameter);
            }
        }
    }
}
