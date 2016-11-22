using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RPGSite.Models.Dice;

namespace RPGSite.Models
{
    public class HomeViewModel
    {
        public string SelectedId { get; set; }
        public SelectList DiceRollers { get; set; }

        public HomeViewModel(User user)
        {
            DiceRollers = new SelectList(user.DiceRollers,"Id","Name");
        }
    }
}