using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using RPGSite.Models;

namespace RPGSite.Controllers.Hubs
{
    /// <summary>
    /// Hub class for the chat, and connected functionality.
    /// </summary>
    public class ChatHub : Hub
    {
        /// <summary>
        /// First reciever for all messages.
        /// </summary>
        /// <param name="message">The message sent by a client.</param>
        public void Send(string message)
        {
            User user = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(HttpContext.Current.User.Identity.GetUserId()); //TODO: Replace?

            if (message.StartsWith("/"))
            {
                InterpretCommand(message, user); //If message contains a command.
            }
            else
            {
                Clients.All.broadcastMessage(user.UserName + ": " + message); //Basic all chatmessage.
            }
        }
        /// <summary>
        /// Interprets the command, 
        /// </summary>
        /// <param name="message">The message to be interpret.</param>
        /// <param name="user">The user that sent the message.</param>
        public void InterpretCommand(string message, User user)
        {
            //Ex. "/roll 1d10 limited" would roll a d10 once and only show the roll for the gm and player.
            if (message.StartsWith("/roll") || message.StartsWith("/Roll"))
            {
                //TODO: Add character stat rolls.
                DieRoll(message, user);
            }
        }

        private static void DieRoll(string message, User user)
        {
            Regex regex = new Regex(@"/(\d)(?:\s?diff(\d{1,}))?$/g");
            if (regex.IsMatch(message))
            {
                Match match = regex.Match(message);

                int times = Convert.ToInt32(match.Groups[1].Value);

                if (!String.IsNullOrEmpty(match.Groups[2].Value))
                {
                    int diff = Convert.ToInt32(match.Groups[3].Value);
                    user.DiceRollers[0].Roll(times,diff);
                }
                else
                {
                    user.DiceRollers[0].Roll(times);
                }
            }
            else
            {
                regex = new Regex(@"/(\d)d(\d{1,})\s?(?:diff(\d{1,})\s)?(limited)?/g");
                Match match = regex.Match(message);

                int times = Convert.ToInt32(match.Groups[1].Value);
                int value = Convert.ToInt32(match.Groups[2].Value);

                if (!String.IsNullOrEmpty(match.Groups[3].Value))
                {
                    int diff = Convert.ToInt32(match.Groups[3].Value);
                    user.DiceRollers[0].Roll(times, value, diff);
                }
                else
                {
                    user.DiceRollers[0].Roll(times, value);
                }
            }
        }
    }
}