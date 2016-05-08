using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace InternetPhotoAlbum.Models
{
    public class User : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Birthday")]
        public DateTime DateOfBirth { get; set; }

        public byte[] UserPhoto { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }

        public User()
        {
            Ratings = new List<Rating>();
            Photos = new List<Photo>();
        }
    }
}

 