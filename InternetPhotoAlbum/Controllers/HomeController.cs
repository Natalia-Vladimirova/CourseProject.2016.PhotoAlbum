using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using InternetPhotoAlbum.Models;
using System.IO;
using System.Data.Entity;

namespace InternetPhotoAlbum.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext Context
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
        }

        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        public ActionResult Index(string id)
        {
            User user = (from u in Context.Users
                         where u.Id == id
                         select u).FirstOrDefault();

            if (user == null)
            {
                user = (from u in Context.Users
                        where u.UserName == User.Identity.Name
                        select u).FirstOrDefault();
            }

            return View(user);
        }

        public async Task<ActionResult> Photos(int id = 0, string userName = null)
        {
            User currentUser = await UserManager.FindByNameAsync(User.Identity.Name);
            User user = null;

            if (userName != null)
            {
                user = await UserManager.FindByNameAsync(userName);
            }

            if (user == null)
            {
                user = currentUser;
            }

            PhotosModel photosModel = new PhotosModel()
            {
                CurrentPhotoId = id,
                ChosenUser = user,
                Photos = user.Photos,
                CurrentUserRating = (from r in Context.Ratings
                                     where r.PhotoId == id && r.UserId == currentUser.Id
                                     select r).FirstOrDefault()
            };

            return View(photosModel);
        }

        public async Task<ActionResult> SearchPhotos(string photoName, string userName, int currentPhotoId = 0)
        {
            User currentUser = await UserManager.FindByNameAsync(User.Identity.Name);
            User user = null;

            if (userName != null)
            {
                user = await UserManager.FindByNameAsync(userName);
            }

            if (user == null)
            {
                user = currentUser;
            }

            IEnumerable<Photo> photos = (from ph in Context.Photos
                                         where ph.UserId == user.Id && ph.Name.Contains(photoName.Trim())
                                         select ph).ToList();

            PhotosModel photosModel = new PhotosModel()
            {
                CurrentPhotoId = currentPhotoId,
                ChosenUser = user,
                Photos = photos,
                CurrentUserRating = (from r in Context.Ratings
                                     where r.PhotoId == currentPhotoId && r.UserId == currentUser.Id
                                     select r).FirstOrDefault()
            };

            return View("Photos", photosModel);
        }

        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(string firstName, string lastName)
        {
            IEnumerable<User> foundUsers;

            if (firstName.Trim() == string.Empty && lastName.Trim() == string.Empty)
            {
                foundUsers = (from u in Context.Users
                              where u.UserName != User.Identity.Name
                              select u).ToList();
            }
            else if (firstName.Trim() == string.Empty)
            {
                foundUsers = (from u in Context.Users
                              where u.UserName != User.Identity.Name && u.LastName.Contains(lastName.Trim())
                              select u).ToList();
            }
            else if (lastName.Trim() == string.Empty)
            {
                foundUsers = (from u in Context.Users
                              where u.UserName != User.Identity.Name && u.FirstName.Contains(firstName.Trim())
                              select u).ToList();
            }
            else
            {
                foundUsers = (from u in Context.Users
                              where u.UserName != User.Identity.Name && u.FirstName.Contains(firstName.Trim()) &&
                                    u.LastName.Contains(lastName.Trim())
                              select u).ToList();
            }

            return View(foundUsers);
        }

        public ActionResult AddPhoto()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddPhoto(Photo pic, HttpPostedFileBase uploadImage)
        {
            TempData["FoundPhotos"] = null;

            User user = await UserManager.FindByNameAsync(User.Identity.Name);

            if (ModelState.IsValid && uploadImage != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                }

                pic.Image = imageData;
                pic.UserId = user.Id;

                Context.Photos.Add(pic);
                await Context.SaveChangesAsync();
            }

            return RedirectToAction("Photos");
        }

        public ActionResult DeletePhoto(int id = 0)
        {
            Photo photo = (from ph in Context.Photos
                           where ph.PhotoId == id
                           select ph).FirstOrDefault();

            if (photo != null)
            {
                Context.Photos.Remove(photo);
                Context.SaveChanges();
            }

            return RedirectToAction("Photos");
        }

        public ActionResult EditPhoto(int id = 0)
        {
            Photo photo = (from ph in Context.Photos
                           where ph.PhotoId == id
                           select ph).FirstOrDefault();

            if (photo == null)
            {
                return RedirectToAction("Photos");
            }

            return View(photo);
        }

        [HttpPost]
        public ActionResult EditPhoto(Photo model)
        {
            Photo photo = (from ph in Context.Photos
                           where ph.PhotoId == model.PhotoId
                           select ph).FirstOrDefault();

            if (photo == null)
            {
                return RedirectToAction("Photos");
            }

            if (ModelState.IsValid)
            {
                photo.Name = model.Name;
                photo.Description = model.Description;
                Context.Photos.Attach(photo);
                Context.Entry(photo).State = EntityState.Modified;
                Context.SaveChanges();
                return RedirectToAction("Photos/" + photo.PhotoId);
            }

            return View(photo);
        }

        public ActionResult Rate(int id, string userName, int rating)
        {
            User currentUser = (from u in Context.Users
                                where u.UserName == User.Identity.Name
                                select u).FirstOrDefault();
            User user;
            user = (from u in Context.Users
                    where u.UserName == userName
                    select u).FirstOrDefault();

            if (user == null)
            {
                user = currentUser;
            }

            Rating userRating = (from r in Context.Ratings
                                 where r.PhotoId == id && r.UserId == currentUser.Id
                                 select r).FirstOrDefault();

            if (userRating == null)
            {
                userRating = new Rating
                {
                    PhotoId = id,
                    UserId = currentUser.Id,
                    UserRate = rating
                };

                Context.Ratings.Add(userRating);
            }
            else
            {
                userRating.UserRate = rating;
                Context.Ratings.Attach(userRating);
                Context.Entry(userRating).State = EntityState.Modified;
            }

            Context.SaveChanges();

            return RedirectToAction($"Photos/{id}/{userName}");
        }

        public ActionResult RemoveRate(int id, string userName)
        {
            User currentUser = (from u in Context.Users
                                where u.UserName == User.Identity.Name
                                select u).FirstOrDefault();

            Rating userRating = (from r in Context.Ratings
                                 where r.PhotoId == id && r.UserId == currentUser.Id
                                 select r).FirstOrDefault();

            if (userRating != null)
            {
                Context.Ratings.Remove(userRating);
                Context.SaveChanges();
            }

            return RedirectToAction($"Photos/{id}/{userName}");
        }

        public ActionResult UserSettings()
        {
            User user = (from u in Context.Users
                         where u.UserName == User.Identity.Name
                         select u).FirstOrDefault();

            if (user == null)
            {
                return RedirectToAction("Index");
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult UserSettings(User model, HttpPostedFileBase uploadImage, string removePhoto)
        {
            User user = (from u in Context.Users
                         where u.UserName == User.Identity.Name
                         select u).FirstOrDefault();

            if (user == null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.DateOfBirth = model.DateOfBirth;

                if (uploadImage != null)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                    }
                    user.UserPhoto = imageData;
                }

                if (removePhoto != null)
                {
                    user.UserPhoto = null;
                }

                Context.Users.Attach(user);
                Context.Entry(user).State = EntityState.Modified;
                Context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

    }
}