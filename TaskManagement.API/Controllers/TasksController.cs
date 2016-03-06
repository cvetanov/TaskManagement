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

        public TasksController(IUnitOfWork uow)
        {
            this._uow = uow as UnitOfWork;
        }
        
        [HttpPost]
        public IHttpActionResult Create(TaskViewModel t)
        {
            var user = _uow.UserProfilesRepository.Get(u => u.Username == t.CreatorUsername).FirstOrDefault();
            Task task = new Task()
            {
                Name = t.Name,
                Description = t.Description,
                OwnerId = user.Id
            };
            _uow.TaskRepository.Add(task);
            _uow.Save();

            return Ok(task);
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var username = (User.Identity as ClaimsIdentity).Claims.Where(c => c.Type == "sub").FirstOrDefault().Value;
                var userId = _uow.UserProfilesRepository.Get(u => u.Username == username).First().Id;
                var tasks = _uow.TaskRepository.Get(t => t.OwnerId == userId).ToList();
                return Ok(tasks);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var username = (User.Identity as ClaimsIdentity).Claims.Where(c => c.Type == "sub").FirstOrDefault().Value;
            var userId = _uow.UserProfilesRepository.Get(u => u.Username == username).First().Id;
            var task = _uow.TaskRepository.Get(id);
            if (task.OwnerId == userId)
            {
                return Ok(task);
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
        }

        [HttpPut]
        public IHttpActionResult Update(Task t)
        {
            var username = (User.Identity as ClaimsIdentity).Claims.Where(c => c.Type == "sub").FirstOrDefault().Value;
            var userId = _uow.UserProfilesRepository.Get(u => u.Username == username).First().Id;
            if (t.OwnerId == userId)
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
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var task = _uow.TaskRepository.Get(id);
                _uow.TaskRepository.Remove(task);
                _uow.Save();
                return Ok();
            }
            catch(Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}
