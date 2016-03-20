using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagement.API.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int CreatedTasks { get; set; }
        public int OtherTasks { get; set; }
        public int FinishedOwnTasks { get; set; }
        public int FinishedOtherTasks { get; set; }
        public int Comments { get; set; }
        public int Friends { get; set; }
        public bool IsFriend { get; set; }
        public DateTime FriendSince { get; set; }
        public bool FriendRequestSent { get; set; }
        public bool FriendRequestReceived { get; set; }
    }
}