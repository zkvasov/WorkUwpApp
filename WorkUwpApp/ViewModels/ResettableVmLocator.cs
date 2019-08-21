using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkUwpApp.ViewModels
{
    public class ResettableVmLocator
    {
        private static object viewModelLocator = new ViewModelLocator();
        public static object ViewModelLocator
        {
            get { return ResettableVmLocator.viewModelLocator; }
        }
    }
}
