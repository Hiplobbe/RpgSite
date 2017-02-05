using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Provider;
using RPGSite.Models;
using RPGSite.Models.Character;
using RPGSite.Models.ViewModels;
using RPGSite.Tools;

namespace RPGSite.Controllers
{
    [Authorize]
    public class CharacterController : Controller
    {
        private RpgContext context = RpgContext.Create();

        #region Character
        public ActionResult AddCharacter()
        {
            return View("Character", new CharacterViewModel(null,GetSheets()));
        }
        public ActionResult EditCharacter(int Id)
        {
            var user = GetUser();

            if (user.Characters.Any(dr => dr.Id == Id))
            {
                return View("Character", new CharacterViewModel(user.Characters.First(dr => dr.Id == Id), GetSheets()));
            }

            return View("Index", new HomeViewModel(user));
        }
        public ActionResult DeleteCharacter(int Id)
        {
            var user = GetUser();
            Character character = user.Characters.FirstOrDefault(dr => dr.Id == Id);

            if (character != null)
            {
                user.Characters.Remove(character);
                context.Characters.Remove(character);

                context.SaveChanges();
            }

            return View("Index", new HomeViewModel(user));
        }
        //public ActionResult SaveCharacter(DiceRoller RollerModel)
        //{
        //    var user = GetUser();

        //    if (!user.DiceRollers.Any(dr => dr.Id == RollerModel.Id))
        //    {
        //        user.DiceRollers.Add(new DiceRoller
        //        {
        //            Name = RollerModel.Name,
        //            StandardValue = RollerModel.StandardValue,
        //            StandardDifficulty = RollerModel.StandardDifficulty,
        //            AgainRule = RollerModel.AgainRule,
        //            AgainValue = RollerModel.AgainValue
        //        });
        //    }
        //    else
        //    {
        //        RollerModel.User = user;
        //        DiceRoller oldDR = context.DiceRollers.First(dr => dr.Id == RollerModel.Id);

        //        context.Entry(oldDR).CurrentValues.SetValues(RollerModel);
        //    }

        //    context.SaveChanges();
        //    return View("Index", new HomeViewModel(GetUser()));
        //}
        #endregion

        public ActionResult AddSheet()
        {
            return View("Sheet",new SheetViewModel(GetAttributes()));
        }

        public ActionResult AddStandardSheet(Sheet sheet)
        {
            if(!context.Sheets.Any(sh => sh.Id == sheet.Id))
            {
                context.Sheets.Add(sheet);
            }

            context.SaveChanges();
            return View("Index");
        }

        public ActionResult AddAttribute()
        {
            return View("Attribute",new AttributeViewModel());
        }
        public string SaveStandardAttribute(Models.Character.Attribute attribute)
        {
            if (!context.Attributes.Any(a => a.Id == attribute.Id))
            {
                context.Attributes.Add(attribute);
            }
            else
            {
                //TODO: Edit old, should be done from main page.
            }

            context.SaveChanges();
            return GetAttributes().ToJson();
        }

        public string SaveAttributeGroup(string name)
        {
            if (!context.AttributeGroups.Any(ag => ag.Name == name))
            {
                context.AttributeGroups.Add(new AttributeGroup(name));

                context.SaveChanges();
                return context.AttributeGroups.First(ag => ag.Name == name).ToJson();
            }

            return null;
        }

        /// <summary>
        /// Gets all standard sheets, for quick creation of new characters.
        /// 
        /// To edit values for characters skills, a user has to edit an existing character.
        /// </summary>
        private SelectList GetSheets()
        {
            return new SelectList(context.Sheets.ToList(), "Id", "Name");
        }
        /// <summary>
        /// Gets all the standard attributes, for easier creation of new sheets.
        /// </summary>
        private SelectList GetAttributes()
        {
            return new SelectList(context.Attributes.Where(a => a.IsStandard).Include(a => a.Type).OrderBy(a => a.Type.Name).ThenBy(a => a.Name), "Id", "Name", "Type.Name", null,null);
        }
        private User GetUser()
        {
            string id = User.Identity.GetUserId();
            User user = context.Users
                .Include(u => u.DiceRollers)
                .Include(u => u.Characters)
                .First(u => u.Id == id);
            return user;
        }
    }
}