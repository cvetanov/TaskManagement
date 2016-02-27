using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagement.ViewModels
{
    public class TaskViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal PercentageDone { get; set; }
        public string CreatorUsername { get; set; }
    }
}