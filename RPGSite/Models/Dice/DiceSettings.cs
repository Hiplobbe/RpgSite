using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPGSite.Models.Dice
{
    public class DiceSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StandardValue { get; set; }
        public int StandardDifficulty { get; set; }
        public bool AgainRule { get; set; }
        public int AgainValue { get; set; }

        
        public virtual DiceRoller DiceRoller { get; set; }
    }
}