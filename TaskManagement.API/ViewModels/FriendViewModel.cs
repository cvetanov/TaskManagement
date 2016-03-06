using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagement.API.ViewModels
{
    public class FriendViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int SentRequest { get; set; }
    }
}