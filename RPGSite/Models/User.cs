using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RPGSite.Models.Card;
using RPGSite.Models.Dice;

namespace RPGSite.Models
{
    //TODO: Add sheets,dice roller, and card dealers.
    //TODO: Optimise usage of the lists.
    public class User : IdentityUser
    {
        public List<CardDealer> CardDealers { get; set; }
        public List<DiceRoller> DiceRollers { get; set; }

        public User()
        {
            CardDealers = new List<CardDealer>();
            DiceRollers = new List<DiceRoller>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}