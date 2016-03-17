using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagement.API.ViewModels
{
    public class CommentViewModel
    {
        public int TaskId { get; set; }
        public string Text { get; set; }
    }
}