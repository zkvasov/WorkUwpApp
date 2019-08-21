using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using WorkUwpApp.Models;

namespace WorkUwpApp.ViewModels
{
    public class Scenario2ViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private ImagesCollection _collection;
        public RelayCommand NavigateCommand { get; private set; }
        private bool _isLoading = false;
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
        private string _title;
        public string Title
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

        public Scenario2ViewModel(INavigationService navigationService, object param)//TODO
        {
            _navigationService = navigationService;
            _collection = (ImagesCollection)param;
            Title = "Image collections";
            NavigateCommand = new RelayCommand(NavigateCommandAction);
        }
        
        private void NavigateCommandAction() //TODO
        {
            // Do Something
        }

        protected void OnNavigatedTo(NavigationEventArgs e)
        {

        }
    }
}
