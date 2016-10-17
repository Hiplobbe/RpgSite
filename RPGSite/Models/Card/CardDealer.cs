using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI;
using Microsoft.Ajax.Utilities;

namespace RPGSite.Models.Card
{
    public class CardDealer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }

        public List<Card> Deck { get; set; }
        public virtual User User { get; set; }

        [NotMapped]
        public List<Card> Discard { get; set; }

        #region Methods
        public void AddCardToDiscard(Card c, bool shuffle)
        {
            Discard.Add(c);

            if (shuffle)
            {
                ShuffleDiscard();
            }
        }
        public void AddCardToDeck(Card c,bool shuffle)
        {
            Deck.Add(c);

            if (shuffle)
            {
                ShuffleDeck();
            }
        }
        public void ShuffleDiscardIntoDeck()
        {
            Deck.AddRange(Discard);
            ShuffleDeck();
            Discard.Clear();
        }
        public void ShuffleDeck()
        {
            Deck = Shuffle(Deck);
        }
        public void ShuffleDiscard()
        {
            Discard = Shuffle(Discard);
        }
        private List<Card> Shuffle(List<Card> list)
        {
            Random rand = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                Card c = list[k];
                list[k] = list[n];
                list[n] = c;
            }

            return list;
        }
        /// <summary>
        /// Changes the suit value for a suit. (On all cards)
        /// </summary>
        /// <param name="Suit">The suit for which the value should be changed.</param>
        /// <param name="value">The new value.</param>
        public void ChangeSuitValue(string Suit, int value)
        {
            foreach (Card card in Deck.FindAll(x => x.Suit == Suit))
            {
                card.SuitValue = value;
            }

            if (Discard.Count > 0)
            {
                foreach (Card card in Discard.FindAll(x => x.Suit == Suit))
                {
                    card.SuitValue = value;
                }
            }
        }
        /// <summary>
        /// Makes a standard deck of cards, with or without jokers.
        /// </summary>
        /// <param name="WithJokers"> Wheter or not the deck should have jokers.</param>
        /// <returns>A list of the ´cards in the deck.</returns>
        public List<Card> MakeStandardDeck(bool WithJokers)
        {
            List<Card> returnList = new List<Card>();

            for (int i = 1; i < 4; i++)
            {
                string suit = "";
                switch (i)
                {
                    case 1:
                        suit = "Diamonds";
                        break;
                    case 2:
                        suit = "Hearts";
                        break;
                    case 3:
                        suit = "Spades";
                        break;
                    case 4:
                        suit = "Clubs";
                        break;
                }

                for (int j = 1; j < 13; j++)
                {
                    returnList.Add(new Card(j,suit,null));
                }

                if (WithJokers)
                {
                    returnList.Add(new Card(14, "Joker", null));
                    returnList.Add(new Card(14, "Joker", null));
                }
            }

            return returnList;
        }
        #endregion
    }
}