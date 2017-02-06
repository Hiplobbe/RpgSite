using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RPGSite.Models
{
    public class CharacterViewModel
    {
        public Character.Character Character { get; set; }
        public int SelectedSheetId { get; set; }
        public SelectList SheetList { get; set; }

        public CharacterViewModel(Character.Character character, SelectList list)
        {
            Character = character;
            SheetList = list;
        }

        public CharacterViewModel(SelectList list)
        {
            Character = new Character.Character();
            SheetList = list;
        }

        public CharacterViewModel()
        {
            
        }
    }
}