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
using RPGSite.Models.Character;
using RPGSite.Models.Wiki;

namespace RPGSite.Models
{
    public class RpgContext : IdentityDbContext<User>
    {
        public DbSet<Character.Attribute> Attributes { get; set; }
        public DbSet<Character.Character> Characters { get; set; }
        public DbSet<AttributeGroup> AttributeGroups { get; set; }
        public DbSet<DiceRoller> DiceRollers { get; set; }
        public DbSet<WikiEntry> WikiEntries { get; set; }
        public DbSet<Sheet> Sheets { get; set; }

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
            modelBuilder.Entity<User>().HasMany(u => u.DiceRollers).WithRequired(dr => dr.User);
            modelBuilder.Entity<User>().HasMany(u => u.CardDealers).WithRequired(cd => cd.User);
            modelBuilder.Entity<User>().HasMany(u => u.Characters).WithRequired(c => c.PlayerUser);

            modelBuilder.Entity<Character.Character>().HasRequired(c => c.PlayerUser);
            modelBuilder.Entity<Character.Character>().HasRequired(c => c.Sheet);

            modelBuilder.Entity<Sheet>().HasMany(s => s.Attributes).WithMany(a => a.Sheets);

            modelBuilder.Entity<AttributeGroup>().HasMany(s => s.Attributes).WithRequired(a => a.Type);

            modelBuilder.Entity<CardDealer>()
                .Property(c => c.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Card.Card>()
                .Property(c => c.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

        public static RpgContext Create()
        {
            return new RpgContext();
        }
    }

    public class DbInitializer : DropCreateDatabaseAlways<RpgContext>
    {
        //TODO: Seed standards.(Dice,Card,Sheets)
        protected override void Seed(RpgContext context)
        {
            context.WikiEntries.Add(new WikiEntry("Index")); //To act as the "starter page" for the wiki.
            SeedRoles(context);
            SeedAdmin(context);
            SeedAttributeGroups(context);

            base.Seed(context);
        }
        /// <summary>
        /// Creates some basic attribute groups.
        /// </summary>
        private void SeedAttributeGroups(RpgContext context)
        {
            context.AttributeGroups.Add(new AttributeGroup("Attributes"));
            context.AttributeGroups.Add(new AttributeGroup("Abilities"));
            context.AttributeGroups.Add(new AttributeGroup("Skills"));
            context.AttributeGroups.Add(new AttributeGroup("Traits"));
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