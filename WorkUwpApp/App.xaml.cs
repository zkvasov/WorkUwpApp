using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Json;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WorkUwpApp.Models;
using WorkUwpApp.Views;

namespace WorkUwpApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private const string _containerCollections= "collsContainer";
        private const string _containerName = "collContainer";
        private const string _collectionNameKey = "collNameKey";
        private const string _imageKey = "imageKey";

        public static string typeNameCurrentPage;
        public static List<ImagesCollection> Collections = new List<ImagesCollection>();

        //Application app;
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        /// 

        public static string CollectionInBg;

        public App()
        {
            //LoadDataAsync();
            DeserializeDataAsync();
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
            
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

#pragma warning disable CA1062 // Validate arguments of public methods
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
#pragma warning restore CA1062 // Validate arguments of public methods
                {
                    //TODO: Load state from previously suspended application
                }

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
                    //TODO

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

                    //

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
        private static void SaveData()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.DeleteContainer(_containerCollections);
            localSettings.CreateContainer(
                _containerCollections, ApplicationDataCreateDisposition.Always);
            if (localSettings.Containers.ContainsKey(_containerCollections))
            {
                int countCollection = 0;
                foreach (var collection in Collections)
                {
                    string colNameToken = _collectionNameKey + countCollection.ToString();
                    localSettings.Containers[_containerCollections].Values[colNameToken] = collection.Name;

                    string collectionToken = _containerName + countCollection.ToString();
                    localSettings.Containers[_containerCollections].DeleteContainer(collectionToken);
                    localSettings.Containers[_containerCollections].CreateContainer(
                        collectionToken, ApplicationDataCreateDisposition.Always);
                    if (localSettings.Containers[_containerCollections].Containers.ContainsKey(collectionToken))
                    {
                        int countImage = 0;
                        foreach (var file in collection.Images)
                        {
                            string fileToken = StorageApplicationPermissions.FutureAccessList.Add(file);
                            string imageToken = _imageKey + countImage.ToString();
                            localSettings.Containers[_containerCollections].Containers[collectionToken].Values[imageToken] = fileToken;
                            countImage++;
                        }
                        countCollection++;
                    }
                }
            }
        }

        /// <summary>
        /// For each collection
        /// Get Name of collection and files(with image) from FutureAccessList
        /// </summary>
        private async void LoadDataAsync()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Containers.ContainsKey(_containerCollections))
            {
                int countCollection = 0;
                foreach (var key in localSettings.Containers[_containerCollections].Containers)
                {
                    string colNameToken = _collectionNameKey + countCollection.ToString();
                    string nameCollection = (string)localSettings.Containers[_containerCollections].Values[colNameToken];
                    var collection = new ImagesCollection(nameCollection);
                    Collections.Add(collection);

                    string collectionKey = _containerName + countCollection.ToString();
                    int countImage = 0;
                    foreach (var token in localSettings.Containers[_containerCollections].Containers[collectionKey].Values)
                    {
                        string imageKey = _imageKey + countImage.ToString();
                        if (localSettings.Containers[_containerCollections].Containers[collectionKey].Values.ContainsKey(imageKey))
                        {
                            string tokenFile = (string)localSettings.Containers[_containerCollections].Containers[collectionKey].Values[imageKey];
                            StorageFile file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(tokenFile);
                            if (file != null)
                            {
                                Collections[countCollection].AddImage(file);
                                // Collections.Add(file);
                            }
                        }

                        countImage++;
                    }
                    countCollection++;
                }
            }
        }
        public static void InsertCollection(int index, ImagesCollection collection)
        {
            Collections.RemoveAt(index);
            Collections.Insert(index, collection);
        }

        private async static void SerializeDataAsync()
        {
            if (Collections.Count > 0)
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await localFolder.CreateFileAsync("ImageCollections.json",
                                                    CreationCollisionOption.ReplaceExisting);
                //DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<ImagesCollection>));
                //using (FileStream fs = new FileStream(file.Path, FileMode.OpenOrCreate))
                //{
                //    jsonFormatter.WriteObject(fs, Collections);
                //}

                string data = JsonConvert.SerializeObject(Collections);
                await FileIO.WriteTextAsync(file, data);
            }
        }

        private async static void DeserializeDataAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.GetFileAsync("ImageCollections.json");
            //StorageFile file = await localFolder.CreateFileAsync("ImageCollections.json",
            //                                       CreationCollisionOption.OpenIfExists);
            string jsonString = await FileIO.ReadTextAsync(file);
            if (!String.IsNullOrEmpty(jsonString))
            {
                Collections = JsonConvert.DeserializeObject<List<ImagesCollection>>(jsonString);
            }
        }
    }
}
