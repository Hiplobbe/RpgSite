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
using MySql.Data.MySqlClient.Memcached;
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
        //TODO: Add message coloring
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
                InterpretCommand(message, ChatUser); //If message contains a command.
            }
            else
            {
                Clients.All.receiveMessage(new HubMessage(ChatUser.DisplayName,message,MessageType.StandardMessage).ToJson()); //Basic all chat message.
            }
        }
        /// <summary>
        /// Interprets the command.
        /// </summary>
        /// <param name="message">The message to be interpret.</param>
        /// <param name="user">The user that sent the message.</param>
        private void InterpretCommand(string message, ChatUser user)
        {
            //Ex. "/roll 1d10 limited" would roll a d10 once and only show the roll for the gm and player.
            if (message.ToLower().StartsWith("/roll"))
            {
                //TODO:Make dicerollers selectable
                //TODO: Add character stat rolls.
                DiceRoller diceRoller = user.GetSelectedRoller();
                if (diceRoller != null)
                {
                    DieRoll(message, user,diceRoller);
                }
            }
            //Changes the display name of the user
            else if (message.ToLower().StartsWith("/nick"))
            {
                ChangeDisplayName(message, user);
            }
            //Sends a message to a user
            else if (message.ToLower().StartsWith("/tell"))
            {
                WhisperUser(message, user);
            }

            //TODO: Add character selection.
        }
        /// <summary>
        /// Sends a private message to another user
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="user">The user sending it.</param>
        private void WhisperUser(string message, ChatUser user)
        {
            Regex regex = new Regex(@"\/tell (\w+) (.*)$");

            Match match = regex.Match(message);

            if (!String.IsNullOrEmpty(match.Groups[1].Value))
            {
                string receiverName = match.Groups[1].Value;

                if (!String.IsNullOrEmpty(match.Groups[2].Value))
                {
                    ChatUser recUser = _LoggedInUsers.FirstOrDefault(u => u.DisplayName == receiverName);

                    if (recUser != null)
                    {
                        Clients.Client(recUser.ConnectionId).receiveMessage(new HubMessage(user.DisplayName, match.Groups[2].Value, MessageType.Whisper));
                    }
                    else
                    {
                        Clients.Client(user.ConnectionId).receiveMessage(new HubMessage("System", "User " + receiverName + " was not found",MessageType.System));
                    }
                }
            }
        }
        /// <summary>
        /// Changes the display name for a user.
        /// </summary>
        /// <param name="message">The message sent requesting the change</param>
        /// <param name="chatUser">The user.</param>
        private void ChangeDisplayName(string message, ChatUser chatUser)
        {
            Regex regex = new Regex(@"\/nick (\w+)");

            Match match = regex.Match(message);

            if (!String.IsNullOrEmpty(match.Groups[1].Value))
            {
                //Changes the display name for the user
                _LoggedInUsers.First(u => u.User.Id == chatUser.User.Id).DisplayName = match.Groups[1].Value;
            }

            Clients.Client(chatUser.ConnectionId).receiveMessage(new HubMessage("System", "Your name has now been changed",MessageType.System));
        }
        private void DieRoll(string message, ChatUser chatUser,DiceRoller diceRoller)//TODO: Test.
        {
            List<DiceRoller.DieResult> results = new List<DiceRoller.DieResult>();
            bool limited = false;

            Regex regex = new Regex(@"(\d)(?:\s?diff(\d{1,}))?\s?(limited)?$"); //TODO: Correct all regex in chat hub.
            if (message.ToLower() == "/roll")
            {
                diceRoller.Roll(1);
            }
            else if(regex.IsMatch(message)) //Ex. /roll 1, Rolls the standard values chosen for the die.
            {
                Match match = regex.Match(message);

                int times = Convert.ToInt32(match.Groups[1].Value);

                if (!String.IsNullOrEmpty(match.Groups[2].Value))
                {
                    int diff = Convert.ToInt32(match.Groups[3].Value);
                    results = diceRoller.Roll(times,diff);
                }
                else
                {
                    results = diceRoller.Roll(times);
                }

                if (!String.IsNullOrEmpty(match.Groups[3].Value))
                {
                    limited = true;
                }
            }
            else //Manual roll
            {
                regex = new Regex(@"(\d)d(\d{1,})\s?(?:diff(\d{1,}))?\s?(limited)?");
                Match match = regex.Match(message);

                int times = Convert.ToInt32(match.Groups[1].Value);
                int value = Convert.ToInt32(match.Groups[2].Value);

                if (!String.IsNullOrEmpty(match.Groups[3].Value)) //Ex. /roll 1d20 diff8 limited
                {
                    int diff = Convert.ToInt32(match.Groups[3].Value);
                    results = diceRoller.Roll(times, value, diff);
                }
                else
                {
                    results = diceRoller.Roll(times, value); //Rolls with standard difficulty.
                }

                if(!String.IsNullOrEmpty(match.Groups[4].Value))
                {
                    limited = true;
                }
            }

            SendDieResult(results,chatUser.DisplayName,limited);
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
            public int ChosenDiceRoller { get; set; }//The chosen dice roller.
            public User User { get; set; }

            public ChatUser(User user, string conId)
            {
                DisplayName = user.UserName;
                ConnectionId = conId;
                User = user;
            }
            /// <summary>
            /// Selects a character for easier rolls, and changes the display name for the user.
            /// </summary>
            /// <param name="charactername">The chosen characters name.</param>
            /// <param name="setasdisplay">Wether or not to change the display name for the user in chat.</param>
            public string SelectCharacter(string characterName,bool setasDisplay)
            {
                Character character = User.Characters.First(c => c.Name.ToLower() == characterName.ToLower());

                if (character != null)
                {
                    ChosenCharacterId = character.Id;

                    if (setasDisplay)
                    {
                        DisplayName = characterName;

                        return "You changed name to " + characterName;
                    }

                    return "Your character is now set as " + characterName;
                }

                return "No character found with that name";
            }
            /// <summary>
            /// Selects a dice roller for rolls
            /// </summary>
            /// <param name="rollerId">The dice roller id.</param>
            public void SelectRoller(int rollerId)
            {
                if (User.DiceRollers.Exists(dr => dr.Id == rollerId))
                {
                    ChosenDiceRoller = rollerId;
                }
            }
            /// <summary>
            /// Gets the selected dice roller.
            /// </summary>
            public DiceRoller GetSelectedRoller()
            {
                if (ChosenDiceRoller != 0)
                {
                    return User.DiceRollers.First(dr => dr.Id == ChosenCharacterId);
                }

                return null;
            }
            /// <summary>
            /// Gets the selected character for the player.
            /// </summary>
            /// <returns>A character object if one is chosen, otherwise null.</returns>
            public Character GetSelectedCharacter()
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
            StandardMessage,
            Whisper,
            Roll,
            System,
            Error,
            CardDraw
        }
        #endregion
    }
}