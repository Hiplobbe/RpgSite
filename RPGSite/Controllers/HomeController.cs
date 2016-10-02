using System;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RPGSite.Models;
using RPGSite.Models.Dice;


namespace RPGSite.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private RpgContext context = new RpgContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddDiceRoller()
        {
            return View("Add/Diceroller",new EditViewDiceRoller { SettingsList = SettingsList()});
        }

        public ActionResult AddDiceSettings()
        {
            return View("Add/DiceSettings");
        }

        private IEnumerable<SelectListItem> SettingsList()
        {
            string id = User.Identity.GetUserId();
            User user = context.Users.Single(u => u.Id == id);

            var rollers = user.DiceRollers.Select(dr =>
                new SelectListItem
                {
                    Value = dr.Id.ToString(),
                    Text = dr.Name
                });

            return new SelectList(rollers,"Value","Text");
        }
    }
}