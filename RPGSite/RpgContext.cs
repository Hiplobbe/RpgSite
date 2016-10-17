using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
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

            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });

            modelBuilder.Entity<User>().HasKey<string>(u => u.Id);
            modelBuilder.Entity<User>().HasMany(u => u.DiceSettings).WithRequired(ds => ds.User);
            modelBuilder.Entity<User>().HasMany(u => u.DiceRollers).WithRequired(dr => dr.User);
            modelBuilder.Entity<User>().HasMany(u => u.CardDealers).WithRequired(cd => cd.User);

            modelBuilder.Entity<CardDealer>()
                .Property(c => c.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Card.Card>()
                .Property(c => c.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<DiceRoller>().HasRequired(dr => dr.Settings).WithOptional(ds => ds.DiceRoller);
        }

        public static RpgContext Create()
        {
            return new RpgContext();
        }
    }

    public class DbInitializer : DropCreateDatabaseIfModelChanges<RpgContext>
    {
        //TODO: Seed standards.(Dice,Card,Sheets)
        protected override void Seed(RpgContext context)
        {
            context.WikiEntries.Add(new WikiEntry("Index")); //To act as the "starter page" for the wiki.
            SeedRoles(context);
            SeedAdmin(context);

            base.Seed(context);
        }
        /// <summary>
        /// Creates an admin user, with password "admin123".
        /// </summary>
        private void SeedAdmin(RpgContext context)
        {
            ApplicationUserManager manager = new ApplicationUserManager(new UserStore<User>(context));
            User user = new User { UserName = "Admin"};
            
            manager.Create(user, "admin123");
            manager.AddToRole(user.Id, "GM");
        }
        /// <summary>
        /// Creates two standard roles, GM and Player.
        /// </summary>
        private void SeedRoles(RpgContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            roleManager.Create(new IdentityRole("Player"));
            roleManager.Create(new IdentityRole("GM"));
        }
    }
}