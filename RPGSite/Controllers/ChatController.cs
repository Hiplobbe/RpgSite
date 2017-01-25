using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RPGSite.Models;

namespace RPGSite.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        // GET: Chat
        public ActionResult Index()
        {
            return View(new ChatViewModel(GetUser()));
        }

        private User GetUser()
        {
            RpgContext context = new RpgContext();

            string id = User.Identity.GetUserId();
            User user = context.Users
                .Include(u => u.DiceRollers)
                .Include(u => u.Characters)
                .First(u => u.Id == id);
            return user;
        }
    }
}