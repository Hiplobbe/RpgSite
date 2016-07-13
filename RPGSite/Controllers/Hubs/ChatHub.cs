using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using RPGSite.Models;
using RPGSite.Models.Dice;
using RPGSite.Tools;

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
                Clients.All.broadcastMessage(new HubMessage(user.UserName,message,MessageType.ChatMessage).ToJson()); //Basic all chatmessage.
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

        private void DieRoll(string message, User user)//TODO: Test.
        {
            List<DiceRoller.DieResult> results = new List<DiceRoller.DieResult>();
            bool limited = false;

            Regex regex = new Regex(@"/(\d)(?:\s?diff(\d{1,}))?\s?(limited)?$/g"); //TODO: Correct all regex in chat hub.
            if (regex.IsMatch(message)) //Ex. /roll 1
            {
                Match match = regex.Match(message);

                int times = Convert.ToInt32(match.Groups[1].Value);

                if (!String.IsNullOrEmpty(match.Groups[2].Value))
                {
                    int diff = Convert.ToInt32(match.Groups[3].Value);
                    results = user.DiceRollers[0].Roll(times,diff);
                }
                else
                {
                    results = user.DiceRollers[0].Roll(times);
                }

                if (!String.IsNullOrEmpty(match.Groups[3].Value))
                {
                    limited = true;
                }
            }
            else
            {
                regex = new Regex(@"/(\d)d(\d{1,})\s?(?:diff(\d{1,}))?\s?(limited)?/g");
                Match match = regex.Match(message);

                int times = Convert.ToInt32(match.Groups[1].Value);
                int value = Convert.ToInt32(match.Groups[2].Value);

                if (!String.IsNullOrEmpty(match.Groups[3].Value)) //Ex. /roll 1d20 diff8 limited
                {
                    int diff = Convert.ToInt32(match.Groups[3].Value);
                    results = user.DiceRollers[0].Roll(times, value, diff);
                }
                else
                {
                    results = user.DiceRollers[0].Roll(times, value);
                }

                if(!String.IsNullOrEmpty(match.Groups[4].Value))
                {
                    limited = true;
                }
            }

            SendDieResult(results,user.UserName,limited);
        }
        /// <summary>
        /// Sends the die roll results to the players.
        /// </summary>
        /// <param name="results">The result of the roll(s)</param>
        /// <param name="username">The user who rolled</param>
        /// <param name="limited">Visibility of the result(Limited means only gm and command writer can see the result)</param>
        private void SendDieResult(List<DiceRoller.DieResult> results,string username, bool limited)
        {
            //TODO: Add limited view.(Owner and leader sees roll)
            Clients.All.broadcastMessage(new HubMessage(username, results.ToJson(),MessageType.Roll).ToJson()); //Basic all chatmessage.
        }

        private class HubMessage
        {
            public string SenderUsername { get; set; }
            public string Message { get; set; }
            public MessageType Type { get; set; }

            public HubMessage(string senderUsername, string message, MessageType type)
            {
                SenderUsername = senderUsername;
                Message = message;
                Type = type;
            }
        }
        private enum MessageType
        {
            ChatMessage,
            Roll,
            CardDraw
        }
    }
}