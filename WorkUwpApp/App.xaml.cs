﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WorkUwpApp.Helpers;
using WorkUwpApp.Models;
using WorkUwpApp.Services;
using WorkUwpApp.Views;

namespace WorkUwpApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {

        //private const string _containerCollections= "collsContainer";
        //private const string _containerName = "collContainer";
        //private const string _collectionNameKey = "collName";
        //private const string _imageKey = "imageKey";
        private const string _collsInJsonKey = "CollectionsInJson";
        private const string _isLaunchedKey = "IsLaunchedBg";

        internal static string typeNameCurrentPage;
        internal static List<ImagesCollection> Collections = new List<ImagesCollection>();
        internal static List<ImagesCollection> PurchaseCollections = new List<ImagesCollection>();

        public static LicenseInformation AppLicenseInformation { get; set; } = CurrentAppSimulator.LicenseInformation;


        //Application app;
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        /// 


        public App()
        {
            //LoadDataAsync();
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            //this.LeavingBackground += OnLeavingBackGround;
        }

        

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                //if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                //{
                //    //TODO: Load state from previously suspended application
                //}

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    DeserializeDataAsync();
                    AddonsService.LoadAddons();
                    SetTheme();
                    //TODO Comlete(or remake) BgTask logic
                    ResetBgTask();
                    //

                    if (Collections == null || Collections.Count == 0) 
                    {
                        typeNameCurrentPage = "Scenario1_CreateCollection";
                        rootFrame.Navigate(typeof(Scenario1_CreateCollection), e.Arguments);
                    }
                    else if (Collections.Count == 1)
                    {
                        typeNameCurrentPage = "Scenario2_CollectionEditor";
                        rootFrame.Navigate(typeof(Scenario2_CollectionEditor), Collections[0]);
                    }
                    else
                    {
                        typeNameCurrentPage = "Scenario3_CollectionsList";
                        rootFrame.Navigate(typeof(Scenario3_CollectionsList), e.Arguments);
                    }


                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity

            //SaveData();
            SerializeDataAsync();
            deferral.Complete();
        }


        /// <summary>
        /// For each collection
        /// Save Name and add files(with image) to FutureAccessList
        /// </summary>
        //private static void SaveData()
        //{
        //    ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        //    localSettings.DeleteContainer(_containerCollections);
        //    localSettings.CreateContainer(
        //        _containerCollections, ApplicationDataCreateDisposition.Always);
        //    if (localSettings.Containers.ContainsKey(_containerCollections))
        //    {
        //        int countCollection = 0;
        //        foreach (var collection in Collections)
        //        {
        //            string colNameToken = _collectionNameKey + countCollection.ToString();
        //            localSettings.Containers[_containerCollections].Values[colNameToken] = collection.Name;

        //            string collectionToken = _containerName + countCollection.ToString();
        //            localSettings.Containers[_containerCollections].DeleteContainer(collectionToken);
        //            localSettings.Containers[_containerCollections].CreateContainer(
        //                collectionToken, ApplicationDataCreateDisposition.Always);
        //            if (localSettings.Containers[_containerCollections].Containers.ContainsKey(collectionToken))
        //            {
        //                int countImage = 0;
        //                foreach (var file in collection.Images)
        //                {
        //                    string fileToken = StorageApplicationPermissions.FutureAccessList.Add(file);
        //                    string imageToken = _imageKey + countImage.ToString();
        //                    localSettings.Containers[_containerCollections].Containers[collectionToken].Values[imageToken] = fileToken;
        //                    countImage++;
        //                }
        //                countCollection++;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// For each collection
        /// Get Name of collection and files(with image) from FutureAccessList
        /// </summary>
        //private async void LoadDataAsync()
        //{
        //    ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        //    if (localSettings.Containers.ContainsKey(_containerCollections))
        //    {
        //        int countCollection = 0;
        //        foreach (var key in localSettings.Containers[_containerCollections].Containers)
        //        {
        //            string colNameToken = _collectionNameKey + countCollection.ToString();
        //            string nameCollection = (string)localSettings.Containers[_containerCollections].Values[colNameToken];
        //            var collection = new ImagesCollection(nameCollection);
        //            Collections.Add(collection);

        //            string collectionKey = _containerName + countCollection.ToString();
        //            int countImage = 0;
        //            foreach (var token in localSettings.Containers[_containerCollections].Containers[collectionKey].Values)
        //            {
        //                string imageKey = _imageKey + countImage.ToString();
        //                if (localSettings.Containers[_containerCollections].Containers[collectionKey].Values.ContainsKey(imageKey))
        //                {
        //                    string tokenFile = (string)localSettings.Containers[_containerCollections].Containers[collectionKey].Values[imageKey];
        //                    StorageFile file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(tokenFile);
        //                    if (file != null)
        //                    {
        //                        Collections[countCollection].AddImage(file);
        //                        // Collections.Add(file);
        //                    }
        //                }

        //                countImage++;
        //            }
        //            countCollection++;
        //        }
        //    }
        //}
        public static void InsertCollection(int index, ImagesCollection collection)
        {
            if (index >= 0)
            {
                Collections.RemoveAt(index);
                Collections.Insert(index, collection);
            }
            else
            {
                Collections.Clear();
                Collections.Add(collection);
            }
        }

        private static void SerializeDataAsync()
        {
            //StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            //StorageFile file = await localFolder.CreateFileAsync("ImageCollections.json",
            //                                    CreationCollisionOption.ReplaceExisting);
            string data = JsonConvert.SerializeObject(Collections);

            //await ApplicationData.Current.LocalSettings.SaveAsync(_collsInJsonKey, data);

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values[_collsInJsonKey] = data;

            //await FileIO.WriteTextAsync(file, data);

        }
        //TODO: Delete after testing
        private static void ResetBgTask()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values[_isLaunchedKey] = false;
        }
        private static void DeserializeDataAsync()
        {
            //StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            //if (await localFolder.TryGetItemAsync("ImageCollections.json") != null)
            //{
            //    StorageFile file = await localFolder.GetFileAsync("ImageCollections.json");
            //    string jsonString = await FileIO.ReadTextAsync(file);
            //    if (!String.IsNullOrEmpty(jsonString))
            //    {
            //        Collections = JsonConvert.DeserializeObject<List<ImagesCollection>>(jsonString);
            //    }
            //}

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey(_collsInJsonKey))
            {
                string data = (string)localSettings.Values[_collsInJsonKey];
                if (!String.IsNullOrEmpty(data))
                {
                    Collections = JsonConvert.DeserializeObject<List<ImagesCollection>>(data);
                }
            }

        }

        private async void SetTheme()
        {
            await ThemeSelectorService.InitializeAsync().ConfigureAwait(true);
            await ThemeSelectorService.SetRequestedThemeAsync().ConfigureAwait(false);
        }
    }
}