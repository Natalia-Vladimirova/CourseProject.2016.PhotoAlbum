using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InternetPhotoAlbum.Models
{
    public class Rating
    {
        public string RatingId { get; set; }
        public int UserRate { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }

        public int PhotoId { get; set; }
        public virtual Photo Photo { get; set; }
    }
}