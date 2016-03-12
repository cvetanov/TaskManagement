using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskManagement.API.DataLayer.Models;

namespace TaskManagement.ViewModels
{
    public class TaskViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<UserProfile> UsersInTask { get; set; }
    }
}