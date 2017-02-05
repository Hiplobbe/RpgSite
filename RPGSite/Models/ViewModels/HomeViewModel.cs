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
        public string SelectedDiceId { get; set; }
        public SelectList DiceRollers { get; set; }

        public string SelectedCharId { get; set; }
        public SelectList Characters { get; set; }

        public HomeViewModel(User user)
        {
            DiceRollers = new SelectList(user.DiceRollers,"Id","Name");
            Characters = new SelectList(user.Characters, "Id", "Name");
        }
    }
}