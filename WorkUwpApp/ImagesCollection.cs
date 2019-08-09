using System.Collections.Generic;
using Windows.Storage;

namespace WorkUwpApp
{
    internal class ImagesCollection
    {
        //to do
        private List<StorageFile> _images;

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
        public List<StorageFile> Images => _images;
        
        public string Name { get; private set; }
        //public StorageFolder StorageFolder { get; private set; }

        public void AddImage(StorageFile image)
        {
            _images.Add(image);
        }
    }
}
