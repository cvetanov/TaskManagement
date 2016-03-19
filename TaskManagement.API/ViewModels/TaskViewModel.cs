using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskManagement.API.DataLayer.Models;
using TaskManagement.API.ViewModels;

namespace TaskManagement.API.ViewModels
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public List<FriendViewModel> UsersInTask { get; set; }
    }
}