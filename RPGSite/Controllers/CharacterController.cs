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
using Attribute = RPGSite.Models.Character.Attribute;

namespace RPGSite.Controllers
{
    [Authorize]
    public class CharacterController : Controller
    {
        private RpgContext context = RpgContext.Create();

        #region Character
        public ActionResult AddCharacter()
        {
            return View("Character", new CharacterViewModel(null,GetSheets(GetUser().Id,true)));
        }
        public ActionResult EditCharacter(int charId)
        {
            var user = GetUser();

            if (user.Characters.Any(dr => dr.Id == charId))
            {
                Character character = user.Characters.First(dr => dr.Id == charId);

                return View("Character", new CharacterViewModel(character, GetSheets(character.Sheet)));
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

            return RedirectToAction("Index","Home", new HomeViewModel(user));
        }
        public ActionResult SaveCharacter(CharacterViewModel model)
        {
            var user = GetUser();

            if (model.Character.Id == 0 && model.SelectedSheetId != 0)
            {
                Sheet modelSheet = context.Sheets.First(sh => sh.Id == model.SelectedSheetId);

                model.Character.Sheet = new Sheet
                {
                    Name = model.Character.Name + "'s sheet",
                    UserId = user.Id,
                    Attributes = modelSheet.Attributes,
                    IsStandard = false
                };

                model.Character.UserId = user.Id;

                context.Characters.Add(model.Character);
            }

            context.SaveChanges();
            return RedirectToAction("Index","Home", new HomeViewModel(user));
        }
        #endregion
        #region Sheets
        public ActionResult AddSheet()
        {
            return View("Sheet",new SheetViewModel(GetAttributes()));
        }
        /// <summary>
        /// Called when a user has chosen to make a sheet for an existing character.
        /// </summary>
        /// <param name="charId">The id of the character</param>
        public ActionResult AddSheetForCharacter(int charId)
        {
            return View("Sheet", new SheetViewModel(charId, GetAttributes()));
        }
        public ActionResult SaveSheet(SheetViewModel model)
        {
            var user = GetUser();
            string returnUrl = "";

            if (model.CharacterId <= 0) //TODO: Only admin can make standard sheets?
            {
                Sheet sheet = model.Sheet;
                sheet.Attributes = new List<Attribute>();

                foreach (int attriId in model.SelectedAttributeIds)
                {
                    Attribute attri = context.Attributes.First(a => a.Id == attriId);

                    sheet.Attributes.Add(attri);
                }

                sheet.UserId = user.Id;

                context.Sheets.Add(sheet);
            }
            else
            {
                //TODO:Edit sheet.
            }

            context.SaveChanges();

            if (model.CharacterId == 0)
            {
                return RedirectToAction("AddCharacter", "Character");
            }
            else if (model.CharacterId > 0)
            {
                return RedirectToAction("EditCharacter", "Character", model.CharacterId);
            }
            else
            {
                return RedirectToAction("Index", "Home", new HomeViewModel(user));
            }
            
        }
        #endregion
        #region Attributes
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
        #endregion
        #region Private methods
        /// <summary>
        /// Gets all standard sheets, for quick creation of new characters.
        /// 
        /// To edit values for characters skills, a user has to edit an existing character.
        /// </summary>
        private SelectList GetSheets(string userId, bool onlyStandard)
        {
            if (onlyStandard)
            {
                return new SelectList(context.Sheets.Where(sh => sh.IsStandard), "Id", "Name");
            }
            else
            {
                return new SelectList(context.Sheets.Where(sh => sh.UserId == userId), "Id", "Name");
            }
        }
        /// <summary>
        /// Gets all the standard sheets and adds the characters own sheet to the list. 
        /// </summary>
        /// <param name="charSheet">The sheet from the character.</param>
        /// <returns></returns>
        private SelectList GetSheets(Sheet charSheet)
        {
            List<Sheet> sheetList = context.Sheets.Where(sh => sh.IsStandard).ToList();
            sheetList.Add(charSheet);

            return new SelectList(sheetList, "Id", "Name");
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
        #endregion
    }
}