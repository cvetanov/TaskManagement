using TaskManagement.Models;
using TaskManagement.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaskManagement.API.DataLayer.Models;
using System.Web.Security;
using System.Security.Claims;
using TaskManagement.API.ViewModels;
using TaskManagement.API;

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
                    UserId = u.Id
                };
                _uow.UsersInTasksRepository.Add(userinTask);
            });
            
            _uow.Save();
            _notifyRefreshNew(task);
            return Ok(task);
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            var myTasks = _uow.TaskRepository.Get(t => t.OwnerId == _userId).OrderBy(t => t.Status).ToList();
            var otherTasks = _uow.TaskRepository.Get(t => t.UsersInTasks.Where(u => u.TaskId == t.Id && u.Active.Value && u.UserId.Value == _userId).FirstOrDefault() != null).OrderBy(t => t.Status).ToList();

            var tasks = new
            {
                myTasks = myTasks,
                otherTasks = otherTasks
            };

            return Ok(tasks);
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var task = _uow.TaskRepository.Get(id);
            // user is not owner and is not in task -> can't view this task
            if (task.OwnerId != _userId && !task.UsersInTasks.Select(u => u.UserId).Contains(_userId))
            {
                return NotFound();
            }
            task.Comments = task.Comments.OrderBy(c => c.Id).ToList();
            return Ok(task);
        }

        [HttpPut]
        public IHttpActionResult Update(TaskViewModel t)
        {
            var task = _uow.TaskRepository.Get(t.Id);

            // user is not the owner and is not in task
            if (task.OwnerId != _userId && !task.UsersInTasks.Select(u => u.UserId).Contains(_userId))
            {
                return NotFound();
            }

            // task is closed, can't update
            if (task.Status.Value)
            {
                return NotFound();
            }
            
            // if task is not closed, but viewmodel has status true (close)
            if (!task.Status.Value && t.Status)
            {
                task.Status = true;
                task.UserClosedId = _userId;
                _uow.TaskRepository.Update(task);
                _uow.Save();
                
                var usernames = task.UsersInTasks.Select(u => u.User.Username).ToList();
                usernames.Add(task.Owner.Username);
                NotificationHub.NotifyRefreshTask(usernames, task.Id);
                NotificationHub.NotifyRefreshTasks(usernames, task.Id);

                return Ok(task);
            }

            var removedFriends = task.UsersInTasks.Where(u => !t.UsersInTask.Select(ut => ut.Id).Contains(u.UserId.Value)).ToList();
            var removedFriendsUsernames = removedFriends.Select(f => f.User.Username).ToList();
            var friendsToAdd = t.UsersInTask.Where(ut => !task.UsersInTasks.Select(u => u.UserId).Contains(ut.Id)).ToList();

            removedFriends.ForEach(f =>
            {
                var userInTask = _uow.UsersInTasksRepository.Get(u => u.TaskId == task.Id && u.UserId == f.UserId.Value).FirstOrDefault();
                _uow.UsersInTasksRepository.Remove(userInTask);
            });
            _uow.Save();

            friendsToAdd.ForEach(f =>
            {
                var userInTask = new UsersInTask
                {
                    TaskId = task.Id,
                    UserId = f.Id,
                    Active = true,
                    DateStarted = DateTime.Now
                };
                _uow.UsersInTasksRepository.Add(userInTask);
            });
            _uow.Save();

            // load task again with changes
            task = _uow.TaskRepository.Get(t.Id);
            task.Name = t.Name;
            task.Description = t.Description;

            _uow.TaskRepository.Update(task);
            _uow.Save();

            // notify users in task
            _notifyRefreshNew(task);
            // notify removed users from task
            NotificationHub.NotifyRefreshTasks(removedFriendsUsernames, task.Id);
            NotificationHub.NotifyRefreshTask(removedFriendsUsernames, task.Id);

            return Ok(task);
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var task = _uow.TaskRepository.Get(id);

            // user is not owner and user is not in task -> user can't delete this task
            if (task.OwnerId != _userId && !task.UsersInTasks.Select(u => u.UserId).Contains(_userId))
            {
                return NotFound();
            }

            var usersInTask = _uow.UsersInTasksRepository.Get(ut => ut.TaskId == task.Id);
            var usernamesToNotify = usersInTask.Select(u => u.User.Username).ToList();
            usernamesToNotify.Add(_uow.UserProfilesRepository.Get(task.OwnerId.Value).Username);
            var taskId = task.Id;

            foreach (UsersInTask ut in usersInTask)
            {
                ut.TaskId = null;
                ut.Active = false;
                _uow.UsersInTasksRepository.Update(ut);
            }
            _uow.TaskRepository.Remove(task);
            _uow.Save();

            NotificationHub.NotifyRefreshTask(usernamesToNotify, taskId);
            NotificationHub.NotifyRefreshTasks(usernamesToNotify, taskId);

            return Ok();
        }

        private void _notifyRefreshNew(Task task)
        {
            var userIds = task.UsersInTasks.Select(u => u.UserId.Value).ToList();
            var usernames = _uow.UserProfilesRepository.Get(u => userIds.Contains(u.Id)).Select(u => u.Username).ToList();
            var ownerUsername = _uow.UserProfilesRepository.Get(task.OwnerId.Value).Username;
            usernames.Add(ownerUsername);
            NotificationHub.NotifyRefreshTask(usernames, task.Id);
            NotificationHub.NotifyRefreshTasks(usernames, task.Id);
        }
    }
}
