using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RPGSite.Models.Dice
{
    public class EditViewDiceRoller
    {
        public string Name { get; set; }
        [Display(Name = "Name")]
        public IEnumerable<SelectListItem> SettingsList { get; set; }
    }
}