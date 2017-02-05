using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RPGSite.Models.Character;

namespace RPGSite.Models.ViewModels
{
    public class AttributeViewModel
    {
        public Character.Attribute Attribute { get; set; }
        public int SelectedGroupId { get; set; }
        public SelectList AttrributeGroupsList =
            new SelectList(RpgContext.Create().AttributeGroups.Where(ag => ag.Id != 0), "Id", "Name");

        public AttributeViewModel(Character.Attribute attri)
        {
            Attribute = attri;
        }
        public AttributeViewModel()
        {

        }
    }
}