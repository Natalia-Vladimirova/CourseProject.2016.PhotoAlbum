using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InternetPhotoAlbum.Models
{
    public class Photo
    {
        public int PhotoId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public byte[] Image { get; set; }

        [Display(Name = "Total rate")]
        public int TotalRate { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }

        public Photo()
        {
            Ratings = new List<Rating>();
        }

    }
}