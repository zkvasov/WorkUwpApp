using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Windows.Storage;
using WorkUwpApp.ViewModels.Helpers;

namespace WorkUwpApp.Models
{
    [DataContract]
    [Serializable]
    public class ImagesCollection : ObservableObject
    {
        //to do
        [DataMember]
        private List<StorageFile> _images;

        //public ImagesCollection() { }
        public ImagesCollection(string name)
        {
            _images = new List<StorageFile>();
            this.Name = name;
        }
        //public ImagesCollection(StorageFolder storageFolder)
        //{
        //    this.Name = storageFolder.Name;
        //    this.StorageFolder = storageFolder;
        //}
        [DataMember]
        public List<StorageFile> Images => _images;

        [DataMember]
        public string Name { get; set; }
        //public StorageFolder StorageFolder { get; private set; }

        public void AddImage(StorageFile image)
        {
            _images.Add(image);
        }
    }
}
