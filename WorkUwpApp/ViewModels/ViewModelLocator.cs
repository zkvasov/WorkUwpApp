using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using WorkUwpApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkUwpApp.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary> 
    public class ViewModelLocator
    {
        public const string Scenario1Key = "Scenario1_CreateCollection";
        public const string Scenario2Key = "Scenario2_CollectionEditor";
        public const string Scenario3Key = "Scenario3_CollectionsList";

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            var nav = new NavigationService();
            nav.Configure(Scenario1Key, typeof(Scenario1_CreateCollection));
            nav.Configure(Scenario2Key, typeof(Scenario2_CollectionEditor));
            nav.Configure(Scenario3Key, typeof(Scenario3_CollectionsList));
            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
            }
            else
            {
                // Create run time view services and models
            }

            //Register your services used here
            SimpleIoc.Default.Register<INavigationService>(() => nav);
            SimpleIoc.Default.Register<Scenario1ViewModel>();
            SimpleIoc.Default.Register<Scenario2ViewModel>();
            SimpleIoc.Default.Register<Scenario3ViewModel>();

        }


        // <summary>
        // Gets the Scenario1 view model.
        // </summary>
        // <value>
        // The Scenario1 view model.
        // </value>
        public static Scenario1ViewModel Scenario1Instance => ServiceLocator.Current.GetInstance<Scenario1ViewModel>();
        // <summary>
        // Gets the Scenario2 view model.
        // </summary>
        // <value>
        // The Scenario2 view model.
        // </value>
        public static Scenario2ViewModel Scenario2Instance => ServiceLocator.Current.GetInstance<Scenario2ViewModel>();
        // <summary>
        // Gets the Scenario3 view model.
        // </summary>
        // <value>
        // The Scenario3 view model.
        // </value>
        public static Scenario3ViewModel Scenario3Instance => ServiceLocator.Current.GetInstance<Scenario3ViewModel>();

        // <summary>
        // The cleanup.
        // </summary>
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }

}
