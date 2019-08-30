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
using WorkUwpApp.Views;

namespace WorkUwpApp.ViewModels
{
    public class Scenario3ViewModel : ViewModelBase, INavigable
    {
        private readonly INavigationService _navigationService;
        private const string _containerName = "imagesContainer";
        private const string _imageSetting = "image";
        private const string _intervalSetting = "interval";
        private const string _containerChangedSetting = "containerChangedSetting";

        //private string _sourcePageTypeFrom;
        private LauncherBgTask _launcher;
        private ImagesCollection _selectedCollection;
        private int _selectedInterval = 5;
        private bool _isLoading = false;
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
            App.Collections.Remove(SelectedCollection);
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
            //TO DO
            localSettings.Values[_intervalSetting] = SelectedInterval;
            //
            localSettings.DeleteContainer(_containerName);
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


        public void OnNavigatedFrom(object sourceType)
        {
            if (sourceType is string)
            {
                App.typeNameCurrentPage = (string)sourceType;
            }
        }
        public void OnNavigatedTo(object parameter)
        {
            if (App.typeNameCurrentPage == "Scenario3_CollectionsList" && App.Collections.Count > 1)
            {
                this.Collections.Clear();
                foreach (var item in App.Collections)
                {
                    this.Collections.Add(item);
                }
            }
            else if (parameter is ImagesCollection collection)
            {
                if (App.typeNameCurrentPage == "Scenario1_CreateCollection")
                {
                    Collections.Add(collection);
                    App.Collections.Add(collection);
                }
                else if (App.typeNameCurrentPage == "Scenario2_CollectionEditor")
                {
                    if (SelectedCollection != null)
                    {
                        int index = Collections.IndexOf(SelectedCollection);
                        Collections.RemoveAt(index);
                        Collections.Insert(index, collection);
                        App.InsertCollection(index, collection);
                    }
                    else if(App.Collections.Count == 1)
                    {
                        Collections.Add(App.Collections[0]);
                    }
                }
            }
            else if (App.typeNameCurrentPage == "Scenario2_CollectionEditor" && App.Collections.Count == 1)
            {
                Collections.Add(App.Collections[0]);
            }
        }
    }
}
