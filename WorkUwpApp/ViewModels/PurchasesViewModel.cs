using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using WorkUwpApp.Models;
using WorkUwpApp.Services;

namespace WorkUwpApp.ViewModels
{
    public class PurchasesViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private AddonCollection _selectedAddon;

        public ObservableCollection<AddonCollection> Addons { get; }

        public RelayCommand ReturnCommand { get; private set; }
        public RelayCommand PurchaseCollectionCommand { get; private set; }

        public PurchasesViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Addons = new ObservableCollection<AddonCollection>();
            LoadAllAddons();
            ReturnCommand = new RelayCommand(Return);
            PurchaseCollectionCommand = new RelayCommand(PurchaseCollection);
        }

        public AddonCollection SelectedAddon
        {
            get => _selectedAddon;
            set
            {
                Set(ref _selectedAddon, value);
                RaisePropertyChanged(nameof(IsSelectedAddon));
            }
        }
        public bool IsSelectedAddon => SelectedAddon != null;

        public bool IsPurchaseBtnEnable => IsSelectedAddon && !SelectedAddon.IsPurchased;

        private void Return()
        {
            //TODO: clear page

            _navigationService.NavigateTo("Scenario3_CollectionsList");
        }

        //TODO purchase function
        private async void PurchaseCollection()
        {
            //to do
            string inAppOfferToken = AddonsService.GetTokenByName(SelectedAddon.Name);
            if (inAppOfferToken != "error")
            {
                if (!App.AppLicenseInformation.ProductLicenses[inAppOfferToken].IsActive)
                {
                    try
                    {
                        PurchaseResults results = await CurrentAppSimulator.RequestProductPurchaseAsync(inAppOfferToken);
                        if (results.Status == ProductPurchaseStatus.Succeeded)
                        {
                            var collection = await AddonsService.CreateAddonCollectionAsync(SelectedAddon.Name).ConfigureAwait(true);
                            App.PurchaseCollections.Add(collection);
                            int index = Addons.IndexOf(SelectedAddon);
                            Addons[index].IsPurchased = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        // The in-app purchase was not completed because an error occurred.
                        Debug.WriteLine($"Purchase has failed \n{ex.Message}");
                        throw;
                    }
                }
            }

            //_navigationService.NavigateTo("Scenario3_CollectionsList", SelectedCollection);
        }

        private async void LoadAllAddons()
        {
            var addonNames = await AddonsService.GetAllAddonsNamesAsync().ConfigureAwait(true);
            foreach(var name in addonNames)
            {
                Addons.Add(new AddonCollection(name));
            }
        }
    }
}
