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
using WorkUwpApp.Helpers;
using WorkUwpApp.Interfaces;
using Windows.UI.Xaml;
using WorkUwpApp.Services;
using System.Windows.Input;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Windows.UI.StartScreen;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel;

namespace WorkUwpApp.ViewModels
{
    public class Scenario3ViewModel : ViewModelBase, INavigable
    {
        private readonly INavigationService _navigationService;
        private const string _containerName = "imagesContainer";
        private const string _imageSetting = "image";
        private const string _intervalSetting = "interval";
        private const string _containerChangedSetting = "containerChangedSetting";

        //private LauncherBgTask _launcher;
        private ImagesCollection _selectedCollection;
        private int _selectedInterval = 5;
        private bool _isLoading = false;
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        
        public ObservableCollection<ImagesCollection> Collections { get; }

        public RelayCommand SettingsClicked { get; private set; }
        public RelayCommand AddNewCollectionClicked { get; private set; }
        public RelayCommand EditCollectionClicked { get; private set; }
        public RelayCommand RemoveCollectionClicked { get; private set; }
        public RelayCommand PlayInBgClicked { get; private set; }

        private ICommand _switchThemeCommand;
        public ICommand SwitchThemeCommand
        {
            get
            {
                if (_switchThemeCommand == null)
                {
                    _switchThemeCommand = new RelayCommand<ElementTheme>(
                        async (param) =>
                        {
                            ElementTheme = param;
                            await ThemeSelectorService.SetThemeAsync(param);
                        });
                }

                return _switchThemeCommand;
            }
        }

        public Scenario3ViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            //_launcher = new LauncherBgTask();
            Collections = new ObservableCollection<ImagesCollection>();
            SettingsClicked = new RelayCommand(ShowSettings);
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
        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { Set(ref _elementTheme, value); }
        }

        private void ShowSettings()
        {
            _navigationService.NavigateTo("Settings");
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
        private async void PlayInBg()
        {
            MarkCollectionInBg();

            await SetLocalSettingsAsync().ConfigureAwait(false);

            LauncherBgTask.LaunhBgTask();

            await TileManager.SendTileNotificationAsync("Background Control", "Collection is playing now:", SelectedCollection.Name).ConfigureAwait(false);
        }
        private async Task SetLocalSettingsAsync()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values[_intervalSetting] = SelectedInterval;
            localSettings.Values[_containerChangedSetting] = true;
            //
            localSettings.DeleteContainer(_containerName);
            localSettings.CreateContainer(
                _containerName, ApplicationDataCreateDisposition.Always);
            if (localSettings.Containers.ContainsKey(_containerName))
            {
                int count = 0;
                foreach (var filePath in SelectedCollection.ImagePaths)
                {
                    var file = await StorageFile.GetFileFromPathAsync(filePath);
                    string fileToken = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);

                    localSettings.Containers[_containerName].Values[String.Format(_imageSetting + count.ToString())] = fileToken;
                    count++;
                }
            }
        }
        private void MarkCollectionInBg()
        {
            foreach(var collection in Collections)
            {
                collection.IsLaunched = false;
            }
            int index = Collections.IndexOf(SelectedCollection);
            Collections[index].IsLaunched = true;


            foreach (var collection in App.Collections)
            {
                collection.IsLaunched = false;
            }
            App.Collections[index].IsLaunched = true;
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
                }
            }
        }
    }
}
