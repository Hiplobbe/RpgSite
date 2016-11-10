using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RPGSite.Models.Edit
{
    public class EditViewUser
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public EditViewUser(string id, string username)
        {
            Id = id;
            Username = username;
            Password = "******";
        }
        public EditViewUser()
        {
            
        }
    }
}