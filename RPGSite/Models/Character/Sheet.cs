﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPGSite.Models.Character
{
    public class Sheet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        [DisplayName("Is a standard sheet?")]
        public bool IsStandard { get; set; } //Used to copy previous sheets, will always be copied.

        public virtual List<Attribute> Attributes { get; set; }
        public virtual User PlayerUser { get; set; }
    }
}