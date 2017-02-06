using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RPGSite.Models.Character;

namespace RPGSite.Models
{
    public class SheetViewModel
    {
        public Sheet Sheet { get; set; }
        public int CharacterId { get; set; }
        public List<int> SelectedAttributeIds { get; set; }
        public SelectList SelectedAttributeList { get; set; }
        public SelectList AttributeList { get; set; }

        public SheetViewModel(Sheet sheet,SelectList attriList )
        {
            Sheet = sheet;
            SelectedAttributeList = new SelectList(sheet.Attributes.OrderBy(a => a.Type.Name).ThenBy(a => a.Name), "Id", "Name", "Type.Name", null, null);
            AttributeList = attriList;
        }

        public SheetViewModel(int charId,SelectList attriList)
        {
            Sheet = new Sheet();
            CharacterId = charId;
            AttributeList = attriList;
        }

        public SheetViewModel(SelectList attriList)
        {
            Sheet = new Sheet();
            CharacterId = -1;
            AttributeList = attriList;
        }

        public SheetViewModel()
        {
            
        }
    }
}