using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaskManagement.API.DataLayer.Models;
using TaskManagement.API.ViewModels;
using TaskManagement.Persistence;

namespace TaskManagement.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/comments")]
    public class CommentsController : ApiController
    {
        private UnitOfWork _uow;
        private int _userId;

        public CommentsController(IUnitOfWork uow)
        {
            this._uow = uow as UnitOfWork;
            var username = User.Identity.Name;
            _userId = _uow.UserProfilesRepository.Get(u => u.Username == username).First().Id;
        }

        [HttpPost]
        public IHttpActionResult Create(CommentViewModel comment)
        {
            var c = new Comment()
            {
                TaskId = comment.TaskId,
                UserId = _userId,
                Text = comment.Text,
                Date = DateTime.Now,
            };
            _uow.CommentsRepository.Add(c);
            _uow.Save();
            return Ok(c);
        }
    }
}
