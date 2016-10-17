using System;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data.Entity;
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
        private RpgContext context = RpgContext.Create();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddDiceSettings()
        {
            return View("Add/DiceSettings");
        }

        public ActionResult AddDiceRoller()
        {
            return View("Add/Diceroller",new EditViewDiceRoller { SettingsList = SettingsList()});
        }

        public ActionResult SaveDiceRoller(EditViewDiceRoller ViewModel)
        {
            var user = GetUser();

            user.DiceRollers.Add(new DiceRoller
            {
                Name = ViewModel.Name,
                Settings = user.DiceSettings.First(dc => dc.Id == ViewModel.SelectedId)
            });

            context.SaveChanges();
            return View("Index");
        }

        public ActionResult SaveDiceSettings(DiceSettings SettingsModel)
        {
            var user = GetUser();

            user.DiceSettings.Add(new DiceSettings
            {
                Name = SettingsModel.Name,
                StandardValue = SettingsModel.StandardValue,
                StandardDifficulty = SettingsModel.StandardDifficulty,
                AgainRule = SettingsModel.AgainRule,
                AgainValue = SettingsModel.AgainValue
            });

            context.SaveChanges();
            return View("Add/Diceroller", new EditViewDiceRoller { SettingsList = SettingsList() });
        }

        private IEnumerable<SelectListItem> SettingsList()
        {
            var user = GetUser();

            if (user.DiceSettings != null)
            {
                var settings = user.DiceSettings.Select(dr =>
                    new SelectListItem
                    {
                        Value = dr.Id.ToString(),
                        Text = dr.Name
                    });

                return new SelectList(settings, "Value", "Text");
            }

            return new List<SelectListItem>();
        }

        private User GetUser()
        {
            string id = User.Identity.GetUserId();
            User user = context.Users.Include(u => u.DiceSettings).First(u => u.Id == id);
            return user;
        }
    }
}