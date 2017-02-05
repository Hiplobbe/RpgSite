using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RPGSite.Models.Character
{
    public class AttributeGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual List<Attribute> Attributes { get; set; }

        public AttributeGroup(string name)
        {
            Name = name;
        }
        public AttributeGroup()
        {

        }
    }
}