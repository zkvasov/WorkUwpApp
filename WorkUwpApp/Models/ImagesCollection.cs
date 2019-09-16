using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Windows.Storage;
using WorkUwpApp.Helpers;

namespace WorkUwpApp.Models
{
    [DataContract]
    [Serializable]
    public class ImagesCollection : ObservableObject
    {
        //to do
        //[DataMember]
        [NonSerialized]
        private List<string> _imagePaths;
        private string _name;
        private bool _isLaunched = false;

        //public ImagesCollection() { }
        public ImagesCollection(string name)
        {
            _imagePaths = new List<string>();
            this.Name = name;
        }
        [DataMember]
        public List<string> ImagePaths => _imagePaths;

        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                Set(ref _name, value);
                OnPropertyChanged(nameof(Name));
            }
        }
        [DataMember]
        public bool IsLaunched
        {
            get => _isLaunched;
            set
            {
                Set(ref _isLaunched, value);
                OnPropertyChanged(nameof(IsLaunched));
            }
        }
        //public StorageFolder StorageFolder { get; private set; }

        public void AddImagePath(string imagePath)
        {
            ImagePaths.Add(imagePath);
        }
    }
}
