using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RPGSite.Models
{
    public class ChatViewModel
    {
        public string SelectedRollerId { get; set; }
        public SelectList DiceRollers { get; set; }

        public string SelectedCharacterId { get; set; }
        public SelectList Characters { get; set; }

        public ChatViewModel(User user)
        {
            DiceRollers = new SelectList(user.DiceRollers, "Id", "Name");
            Characters = new SelectList(user.Characters,"Id","Name");
        }
    }
}