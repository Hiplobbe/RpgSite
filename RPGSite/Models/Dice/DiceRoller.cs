using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace RPGSite.Models.Dice
{
    public class DiceRoller
    {
        [Key]
        public int SettingsId { get; set; }
        [Required]
        public string Name { get; set; }
        public string UserId { get; set; }

        public virtual DiceSettings Settings { get; set; }
        public virtual User User { get; set; }

        /// <summary>
        /// Rolls a die with the specified value, for the given amount of times.
        /// </summary>
        /// <param name="value">The value of the die.</param>
        /// <param name="times">The amount of times to roll the die.</param>
        /// <param name="difficulty">The difficulty to roll against.</param>
        /// <returns>A list of all the roll results.</returns>
        public List<DieResult> Roll(int times, int value,int difficulty)
        {
            return DieRoll(value, times, difficulty);
        }
        public List<DieResult> Roll(int times, int value)
        {
            return DieRoll(value, times, Settings.StandardDifficulty);
        }
        public List<DieResult> Roll(int times)
        {
            return DieRoll(Settings.StandardValue, times, Settings.StandardDifficulty);
        }

        public List<DieResult> FakeRoll(int times)
        {
            return DieRoll((Settings.StandardDifficulty-1), times, Settings.StandardDifficulty);
        }

        private List<DieResult> DieRoll(int value, int times, int difficulty)
        {
            Random rand = new Random();
            List<DieResult> ReturnList = new List<DieResult>();

            for (int i = 0; i < times; i++)
            {
                int roll = rand.Next(value);

                ReturnList.Add(roll >= difficulty ? new DieResult(roll, true) : new DieResult(roll, false));

                if (Settings.AgainRule && roll == Settings.AgainValue)
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