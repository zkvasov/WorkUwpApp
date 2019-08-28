using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkUwpApp.Interfaces
{
    public interface INavigable
    {
       // void OnNavigatedTo(object parameter);

        void OnNavigatedFrom(object sourceType);
        void OnNavigatedTo(object parameter);

    }
}
