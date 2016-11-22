using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace RPGSite.Models.Dice
{
    public class DiceRoller
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int StandardValue { get; set; }
        public int StandardDifficulty { get; set; }
        public bool AgainRule { get; set; }
        public int AgainValue { get; set; }
        public string UserId { get; set; }

        public virtual User User { get; set; }

        /// <summary>
        /// Rolls a die with the specified value, for the given amount of times.
        /// </summary>
        /// <param name="value">The value of the die.</param>
        /// <param name="times">The amount of times to roll the die.</param>
        /// <param name="difficulty">The difficulty to roll against.</param>
        /// <returns>A list of all the roll results.</returns>
        public List<DieResult> Roll(int times, int value, int difficulty)
        {
            return DieRoll(value, times, difficulty);
        }
        public List<DieResult> Roll(int times, int value)
        {
            return DieRoll(value, times, StandardDifficulty);
        }
        public List<DieResult> Roll(int times)
        {
            return DieRoll(StandardValue, times, StandardDifficulty);
        }

        public List<DieResult> FakeRoll(int times)
        {
            return DieRoll((StandardDifficulty - 1), times, StandardDifficulty);
        }

        private List<DieResult> DieRoll(int value, int times, int difficulty)
        {
            Random rand = new Random();
            List<DieResult> ReturnList = new List<DieResult>();

            for (int i = 0; i < times; i++)
            {
                int roll = rand.Next(value);

                ReturnList.Add(roll >= difficulty ? new DieResult(roll, true) : new DieResult(roll, false));

                if (AgainRule && roll == AgainValue)
                {
                    times++;
                }
            }

            return ReturnList;
        }

        public class DieResult
        {
            public int Value { get; set; }
            public bool Success { get; set; }

            public DieResult(int value, bool success)
            {
                Value = value;
                Success = success;
            }
        }
    }
}