using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPGSite.Models.Wiki
{
    public class WikiEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Title { get; set; }
        [Column(TypeName = "mediumtext")]
        public string Text { get; set; }
        public int Referanced { get; set; } //How many times the entry is referanced, used to determine when to delete an entry.

        public WikiEntry(string title)
        {
            Title = title;
            Text = "";
            Referanced = 1;
        }

        public WikiEntry()
        {
            
        }
    }
}