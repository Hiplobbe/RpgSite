using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPGSite.Models.Character
{
    public class Character
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public int SheetId { get; set; }

        public virtual Sheet Sheet { get; set; }
        public virtual User User { get; set; }
    }
}