using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPGSite.Models.Character
{
    //TODO: Placement on sheet.
    public class Attribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        private int Id { get; set; }
        public string Name { get; set; }
        public int MaxValue { get; set; }
        public int Value { get; set; }
    }
    //TODO: Add to model, so compatible with EF(DB).
    public enum AttributeType
    {
        Attribute,
        Skill,
        Trait
    }
}