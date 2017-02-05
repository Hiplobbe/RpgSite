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
        public string SelectedAttriId { get; set; }
        public SelectList SelectedAttributeList { get; set; }
        public SelectList AttributeList { get; set; }

        public SheetViewModel(Sheet sheet,SelectList attriList )
        {
            Sheet = sheet;
            SelectedAttributeList = new SelectList(sheet.Attributes,"Id","Name"); //TODO: Group.
            AttributeList = attriList;
        }

        public SheetViewModel(SelectList attriList)
        {
            Sheet = new Sheet();
            AttributeList = attriList;
        }
    }
}