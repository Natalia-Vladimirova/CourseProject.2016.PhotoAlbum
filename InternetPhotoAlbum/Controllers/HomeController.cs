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

        public async Task<ActionResult> Photos(int id = 0)
        {
            User user = await UserManager.FindByNameAsync(User.Identity.Name);

            List<Photo> photos = (from ph in Context.Photos
                                         where ph.UserId == user.Id
                                         select ph).ToList();

            if (TempData["CurrentPhotoId"] == null)
            {
                ViewBag.CurrentPhotoId = id;
            }
            else
            {
                ViewBag.CurrentPhotoId = TempData["CurrentPhotoId"];
            }
            return View(photos);
        }

        public ActionResult ChangePhotoPosition(int id = 0)
        {
            TempData["CurrentPhotoId"] = id;
            return RedirectToAction("Photos");
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult AddPhoto()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddPhoto(Photo pic, HttpPostedFileBase uploadImage)
        {
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
        public ActionResult UserSettings(User model, HttpPostedFileBase uploadImage)
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

                Context.Users.Attach(user);
                Context.Entry(user).State = EntityState.Modified;
                Context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        [Authorize(Roles = "admin")]
        public ActionResult UsersEdit()
        {
            return View();
        }

    }
}