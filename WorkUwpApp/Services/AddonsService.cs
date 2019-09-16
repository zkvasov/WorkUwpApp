using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using WorkUwpApp.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WorkUwpApp.Services
{
    public static class AddonsService
    {
        public const string catsPackageToken = "CatsPackageToken";
        public const string mountainsPackageToken = "MountainsPackageToken";
        public const string spacePackageToken = "SpacePackageToken";
        public const string sunsetsPackageToken = "SunsetsPackageToken";

        public static async void LoadAddons()
        {
            StorageFolder proxyDataFolder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
            StorageFile proxyFile = await proxyDataFolder.GetFileAsync("test.xml");
            await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile);

            //TODO
            if (App.AppLicenseInformation.ProductLicenses[catsPackageToken].IsActive)
            {
                var collection = await CreateAddonCollectionAsync("Cats").ConfigureAwait(true);
                App.PurchaseCollections.Add(collection);
            }
            if (App.AppLicenseInformation.ProductLicenses[mountainsPackageToken].IsActive)
            {
                var collection = await CreateAddonCollectionAsync("Mountains").ConfigureAwait(true);
                App.PurchaseCollections.Add(collection);
            }
            if (App.AppLicenseInformation.ProductLicenses[spacePackageToken].IsActive)
            {
                var collection = await CreateAddonCollectionAsync("Space").ConfigureAwait(true);
                App.PurchaseCollections.Add(collection);
            }
            if (App.AppLicenseInformation.ProductLicenses[sunsetsPackageToken].IsActive)
            {
                var collection = await CreateAddonCollectionAsync("Sunsets").ConfigureAwait(true);
                App.PurchaseCollections.Add(collection);
            }
        }

        public static async Task<ImagesCollection> CreateAddonCollectionAsync(string nameFolder)
        {
            ImagesCollection collection = new ImagesCollection(nameFolder);
            StorageFolder dataFolder = await Package.Current.InstalledLocation.GetFolderAsync("Add-ons");
            StorageFolder addonFolder = await dataFolder.GetFolderAsync(nameFolder);
            IReadOnlyList<StorageFile> files = await addonFolder.GetFilesAsync();
            foreach (StorageFile file in files)
            {
                collection.AddImagePath(file.Path);
            }
            return collection;
        }

        //
        public static async Task<List<string>> GetAllAddonsNamesAsync()
        {
            StorageFolder dataFolder = await Package.Current.InstalledLocation.GetFolderAsync("Add-ons");
            var folders = await dataFolder.GetFoldersAsync();
            List<string> names = new List<string>();
            foreach (var folder in folders)
            {
                names.Add(folder.Name);
            }
            return names;
        }

        public static string GetTokenByName(string addonName)
        {
            switch (addonName)
            {
                case "Cats":
                    return catsPackageToken;

                case "Mountains":
                    return mountainsPackageToken;

                case "Space":
                    return spacePackageToken;

                case "Sunsets":
                    return sunsetsPackageToken;

                default:
                    return "error";

            }
        }

    }
}