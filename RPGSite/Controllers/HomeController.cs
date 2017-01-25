using System;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
            return View(new HomeViewModel(GetUser()));
        }

        #region Dice roller
        public ActionResult AddDiceRoller()
        {
            return View("AddEdit/DiceRoller");
        }
        public ActionResult EditDiceRoller(int Id)
        {
            var user = GetUser();

            if (user.DiceRollers.Any(dr => dr.Id == Id))
            {
                return View("AddEdit/DiceRoller", user.DiceRollers.First(dr => dr.Id == Id));
            }

            return View("Index", new HomeViewModel(user));
        }
        public ActionResult DeleteDiceRoller(int Id)
        {
            var user = GetUser();
            DiceRoller roller = user.DiceRollers.FirstOrDefault(dr => dr.Id == Id);

            if (roller != null)
            {
                user.DiceRollers.Remove(roller);
                context.DiceRollers.Remove(roller);

                context.SaveChanges();
            }

            return View("Index", new HomeViewModel(user));
        }
        public ActionResult SaveDiceRoller(DiceRoller RollerModel)
        {
            var user = GetUser();

            if (!user.DiceRollers.Any(dr => dr.Id == RollerModel.Id))
            {
                user.DiceRollers.Add(new DiceRoller
                {
                    Name = RollerModel.Name,
                    StandardValue = RollerModel.StandardValue,
                    StandardDifficulty = RollerModel.StandardDifficulty,
                    AgainRule = RollerModel.AgainRule,
                    AgainValue = RollerModel.AgainValue
                });
            }
            else
            {
                RollerModel.User = user;
                DiceRoller oldDR = context.DiceRollers.First(dr => dr.Id == RollerModel.Id);

                context.Entry(oldDR).CurrentValues.SetValues(RollerModel);
            }

            context.SaveChanges();
            return View("Index", new HomeViewModel(GetUser()));
        }
        #endregion

        private User GetUser()
        {
            string id = User.Identity.GetUserId();
            User user = context.Users
                .Include(u => u.DiceRollers)
                .First(u => u.Id == id);
            return user;
        }
    }
}