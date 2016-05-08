using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InternetPhotoAlbum.Models
{
    public class PhotosModel
    {
        public int CurrentPhotoId { get; set; }
        public User ChosenUser { get; set; }
        public IEnumerable<Photo> Photos { get; set; }
    }
}