using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPGSite.Models.Card
{
    //TODO: Check if saved correctly in db.
    public class Card
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int NumValue { get; set; }
        public int SuitValue { get; set; }
        public string Suit { get; set; }
        public string Name { get; set; } //Used for none standard cards(Tarot or game specific)
        public int CardDealerId { get; set; }

        /// <summary>
        /// Constructor for a card.
        /// </summary>
        /// <param name="value">The numeric value of the card.</param>
        /// <param name="suit">The cards suit (Clubs,clover etc)</param>
        /// <param name="name">The name of the card if the isn't a standard deck card.(Leave empty if standard)</param>
        public Card(int value, string suit,string name)
        {
            NumValue = value;
            Suit = suit;
            Name = name;
        }
        /// <summary>
        /// Like standard constructor but sets the suits value.
        /// </summary>
        /// <param name="numValue">The numeric value of the card.</param>
        /// <param name="suitValue">The numeric value of the suit.</param>
        /// <param name="suit">The suit of the card.</param>
        /// <param name="name">The name of the card if the isn't a standard deck card.(Leave empty if standard)</param>
        public Card(int numValue,int suitValue, string suit, string name)
        {
            NumValue = numValue;
            SuitValue = suitValue;
            Suit = suit;
            Name = name;
        }
    }
}