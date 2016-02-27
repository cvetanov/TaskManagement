using TaskManagement.Models;
using TaskManagement.Persistence;
using TaskManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaskManagement.API.DataLayer.Models;

namespace TaskManagement.Controllers
{
    [Authorize]
    [RoutePrefix("api/Tasks")]
    public class TasksController : ApiController
    {
        private UnitOfWork _uow;

        public TasksController(IUnitOfWork uow)
        {
            this._uow = uow as UnitOfWork;
        }

        public IHttpActionResult GetTasksForOwner(string ownerId)
        {
            //var tasks = _repo.GetForOwner(ownerId);
            return Ok();//tasks.ToList());
        }

        public IHttpActionResult Create(TaskViewModel t)
        {
            var users = _uow.UserProfilesRepository.Get();
            var user = _uow.UserProfilesRepository.Get(u => u.Username == t.CreatorUsername).FirstOrDefault();
            Task task = new Task()
            {
                Name = t.Name,
                Description = t.Description,
                PercentageDone = 0,
                OwnerId = user.Id
            };
            _uow.TaskRepository.Add(task);
            _uow.Save();

            return Ok();
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            var tasks = _uow.TaskRepository.Get();
            return Ok(tasks);
        }
    }
}
