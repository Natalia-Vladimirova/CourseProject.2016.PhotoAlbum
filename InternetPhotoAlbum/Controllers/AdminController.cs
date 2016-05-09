using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity.Owin;
using InternetPhotoAlbum.Models;

namespace InternetPhotoAlbum.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
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

        public async Task<ActionResult> UsersEdit()
        {
            
            IEnumerable<User> allUsers = (from u in Context.Users
                                          select u).ToList();

            List<User> users = new List<User>();
            
            foreach (User user in allUsers)
            {
                if (await UserManager.IsInRoleAsync(user.Id, "admin") == false)
                {
                    users.Add(user);
                }
            }

            return View(users);
        }

        public ActionResult EditUser(string id)
        {
            User user = Context.Users.FirstOrDefault(u => u.Id == id);
            return View(user);
        }

        [HttpPost]
        public ActionResult EditUser(User model)
        {
            User user = (from u in Context.Users
                           where u.Id == model.Id
                           select u).FirstOrDefault();

            if (user == null)
            {
                return RedirectToAction("UsersEdit");
            }

            if (ModelState.IsValid)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.DateOfBirth = model.DateOfBirth;
                Context.Users.Attach(user);
                Context.Entry(user).State = EntityState.Modified;
                Context.SaveChanges();
                return RedirectToAction("UsersEdit");
            }

            return View(user);
        }

        public ActionResult DeleteUser(string id)
        {
            User user = Context.Users.FirstOrDefault(u => u.Id == id);
            return View(user);
        }

        [HttpPost]
        public ActionResult DeleteUser(User model)
        {
            User user = (from u in Context.Users
                         where u.Id == model.Id
                         select u).FirstOrDefault();

            if (user != null)
            {
                var ratings = user.Ratings.ToList();
                for (int i = 0; i < ratings.Count; i++)
                {
                    ratings[i].UserId = null;
                    Context.Ratings.Attach(ratings[i]);
                    Context.Entry(ratings[i]).State = EntityState.Modified;
                }

                Context.Users.Remove(user);
                Context.SaveChanges();
            }

            return RedirectToAction("UsersEdit");
        }

    }
}