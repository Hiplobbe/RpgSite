using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity.EntityFramework;
using RPGSite.Models.Card;
using RPGSite.Models.Dice;
using RPGSite.Models.Wiki;

namespace RPGSite.Models
{
    public class RpgContext : IdentityDbContext<User>
    {
        public DbSet<WikiEntry> WikiEntries { get; set; }

        public RpgContext() : base("ConnectionString")
        {
            Database.SetInitializer<RpgContext>(new DbInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });

            modelBuilder.Entity<CardDealer>()
                .Property(c => c.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Card.Card>()
                .Property(c => c.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<DiceSettings>().HasRequired(ds => ds.DiceRoller).WithRequiredDependent(dr => dr.Settings);
        }

        public static RpgContext Create()
        {
            return new RpgContext();
        }
    }

    public class DbInitializer : DropCreateDatabaseIfModelChanges<RpgContext>
    {
        //TODO: Seed admin account?
        //TODO: Seed standards.(Dice,Card,Sheets)

        protected override void Seed(RpgContext context)
        {
            context.WikiEntries.Add(new WikiEntry("Index")); //To act as the "starter page" for the wiki.

            base.Seed(context);
        }
    }
}