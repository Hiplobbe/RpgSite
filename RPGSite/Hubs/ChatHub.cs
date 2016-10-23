using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using RPGSite.Models;
using RPGSite.Models.Character;
using RPGSite.Models.Dice;
using RPGSite.Tools;

namespace RPGSite.Controllers.Hubs
{
    /// <summary>
    /// Hub class for the chat, and connected functionality.
    /// </summary>
    [Authorize]
    public class ChatHub : Hub
    {
        //TODO: Add functionality for private messages.
        //TODO: Add functionality to view chosen character name.(Instead of the users name)

        private static List<ChatUser> _LoggedInUsers = new List<ChatUser>();
        private static RpgContext _Context = RpgContext.Create();

        #region Tasks
        /// <summary>
        /// Adds a new user to the logged in list.
        /// </summary>
        public override Task OnConnected()
        {
            string id = Context.User.Identity.GetUserId();

            if (!_LoggedInUsers.Exists(u => u.User.Id == id))
            {
                _LoggedInUsers.Add(new ChatUser(GetUser(id), Context.ConnectionId));
            }

            return base.OnConnected();
        }
        /// <summary>
        /// Removes user from list when logged out.
        /// </summary>
        public override Task OnDisconnected(bool stopCalled)
        {
            _LoggedInUsers.RemoveAll(cu => cu.ConnectionId == Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }
        /// <summary>
        /// First reciever for all messages.
        /// </summary>
        /// <param name="message">The message sent by a client.</param>
        #endregion
        public void Send(string id,string message)
        {
            ChatUser ChatUser = _LoggedInUsers.First(u => u.User.Id == id);

            if (message.StartsWith("/"))
            {
                InterpretCommand(message, ChatUser.User); //If message contains a command.
            }
            else
            {
                Clients.All.receiveMessage(new HubMessage(ChatUser.DisplayName,message,MessageType.ChatMessage).ToJson()); //Basic all chatmessage.
            }
        }
        /// <summary>
        /// Interprets the command.
        /// </summary>
        /// <param name="message">The message to be interpret.</param>
        /// <param name="user">The user that sent the message.</param>
        public void InterpretCommand(string message, User user)
        {
            //Ex. "/roll 1d10 limited" would roll a d10 once and only show the roll for the gm and player.
            if (message.StartsWith("/roll") || message.StartsWith("/Roll"))
            {
                //TODO:Make dicerollers selectable
                //TODO: Add character stat rolls.
                DieRoll(message, user);
            }

            //TODO: Add character selection.
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
            Clients.All.receiveMessage(new HubMessage(username, results.ToJson(),MessageType.Roll).ToJson()); //Basic all chatmessage.
        }
        private User GetUser(string id)
        {
            return _Context.Users
                .Include(u => u.Characters)
                .Include(u => u.DiceRollers)
                .Include(u => u.CardDealers)
                .First(u => u.Id == id);
        }

        #region Classes
        private class ChatUser
        {
            public string DisplayName { get; set; }//The name to be displayed in chat.
            public string ConnectionId { get; set; }
            public int ChosenCharacterId { get; set; }//The chosen character for the player.
            public User User { get; set; }

            public ChatUser(User user, string conId)
            {
                DisplayName = User.UserName;
                ConnectionId = conId;
                User = user;
            }
            /// <summary>
            /// Selects a character for easier rolls, and changes the display name for the user.
            /// </summary>
            /// <param name="characterid">The chosen characters id.</param>
            /// <param name="setasdisplay">Wether or not to change the display name for the user in chat.</param>
            public string SelectCharacter(string charactername,bool setasdisplay)
            {
                Character character = User.Characters.First(c => c.Name.ToLower() == charactername.ToLower());

                if (character != null)
                {
                    ChosenCharacterId = character.Id;

                    if (setasdisplay)
                    {
                        DisplayName = charactername;

                        return "You changed name to " + charactername;
                    }

                    return "Your character is now set as " + charactername;
                }

                return "No character found with that name";
            }
            /// <summary>
            /// Gets the selected character for the player.
            /// </summary>
            /// <returns>A character object if one is chosen, otherwise null.</returns>
            public Character GetPreSelectedCharacter()
            {
                if(ChosenCharacterId != 0)
                {
                    return User.Characters.First(c => c.Id == ChosenCharacterId);
                }

                return null;
            }
        }
        private class HubMessage
        {
            public string Username { get; set; }
            public string Message { get; set; }
            public MessageType Type { get; set; }

            public HubMessage(string username, string message, MessageType type)
            {
                Username = username;
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
        #endregion
    }
}