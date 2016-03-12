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
using System.Web.Security;
using System.Security.Claims;

namespace TaskManagement.Controllers
{
    [Authorize]
    [RoutePrefix("api/Tasks")]
    public class TasksController : ApiController
    {
        private UnitOfWork _uow;
        private int _userId;

        public TasksController(IUnitOfWork uow)
        {
            this._uow = uow as UnitOfWork;
            var username = User.Identity.Name;
            _userId = _uow.UserProfilesRepository.Get(u => u.Username == username).First().Id;
        }
        
        [HttpPost]
        public IHttpActionResult Create(TaskViewModel t)
        {
            Task task = new Task()
            {
                Name = t.Name,
                Description = t.Description,
                OwnerId = _userId,
                Status = false
            };

            _uow.TaskRepository.Add(task);
            _uow.Save();

            t.UsersInTask.ForEach(u =>
            {
                var userinTask = new UsersInTask
                {
                    TaskId = task.Id,
                    DateStarted = DateTime.Now,
                    Active = true,
                    UserId = u.Id,
                    LastChange = DateTime.Now
                };
                _uow.UsersInTasksRepository.Add(userinTask);
            });
            
            _uow.Save();

            return Ok(task);
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            var tasks = _uow.TaskRepository.Get(t => t.OwnerId == _userId).ToList();
            
            //TODO
            // get tasks im in
            // split return result in: mytasks & tasks im in

            return Ok(tasks);
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var task = _uow.TaskRepository.Get(id);
            if (task.OwnerId == _userId)
            {
                return Ok(task);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPut]
        public IHttpActionResult Update(Task t)
        {
            // TODO: fix loading errors
            if (t.OwnerId == _userId)
            {
                var task = _uow.TaskRepository.Get(t.Id);
                task.Name = t.Name;
                task.Description = t.Description;
                
                var usersInTasks = t.UsersInTasks.Select(u => new UsersInTask
                {
                    Id = u.Id,
                    TaskId = u.TaskId,
                    UserId = u.UserId,
                    Active = u.Active,
                    DateStarted = u.DateStarted,
                    LastChange = u.LastChange
                }).ToList();

                task.UsersInTasks.ToList().ForEach(u => _uow.UsersInTasksRepository.Remove(u));
                task.UsersInTasks = usersInTasks;

                _uow.TaskRepository.Update(task);
                _uow.Save();
                return Ok(task);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var task = _uow.TaskRepository.Get(id);
            if (task.OwnerId == _userId)
            {
                var usersInTask = _uow.UsersInTasksRepository.Get(ut => ut.TaskId == task.Id);
                foreach (UsersInTask ut in usersInTask)
                {
                    ut.TaskId = null;
                    ut.LastChange = DateTime.Now;
                    ut.Active = false;
                    _uow.UsersInTasksRepository.Update(ut);
                }
                _uow.TaskRepository.Remove(task);
                
                _uow.Save();
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
