using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkUwpApp.Helpers;


namespace WorkUwpApp.Models
{
    public class AddonCollection : ObservableObject
    {
        private string _name;
        private bool _isPurchased = false;
        public AddonCollection(string name)
        {
            this.Name = name;
        }

        public string Name
        {
            get => _name;
            set
            {
                Set(ref _name, value);
                OnPropertyChanged(nameof(Name));
            }
        }

        public bool IsPurchased
        {
            get => _isPurchased;
            set
            {
                Set(ref _isPurchased, value);
                OnPropertyChanged(nameof(IsPurchased));
            }
        }
    }
}
