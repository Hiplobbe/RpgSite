using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPGSite.Models.Character
{
    //TODO: Sheet editor for a digital "pdf" representation of the sheet.
    public class Attribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }//TODO: Name must be unique?
        public int MaxValue { get; set; }
        public int Value { get; set; }//The value the character has in this skill, will always be 0 for standard attributes.
        public bool IsStandard { get; set; }
        public int AttributeGroupId { get; set; }
        

        public virtual List<Sheet> Sheets { get; set; }
        public virtual AttributeGroup Type { get; set; }
    }
}