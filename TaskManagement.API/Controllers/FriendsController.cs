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
    [RoutePrefix("api/friends")]
    public class FriendsController : ApiControllerWithHub<NotificationHub>
    {
        private UnitOfWork _uow;
        private int _userId;

        public FriendsController(IUnitOfWork uow)
        {
            _uow = uow as UnitOfWork;
            if (User.Identity.IsAuthenticated)
            {
                _userId = _uow.UserProfilesRepository.Get(u => u.Username == User.Identity.Name).First().Id;
            }
            else
            {
                _userId = -1;
            }
        }

        [HttpGet]
        [Route("GetNonFriends")]
        public IHttpActionResult GetNonFriends()
        {
            var friends = _uow.FriendsRepository.Get(f => f.User1.Value == _userId).Select(f => new FriendViewModel
            {
                Id = f.User2.Value,
                Username = f.UserProfile2.Username,
                SentRequest = 0
            }).ToList();
            friends.Add(new FriendViewModel
            {
                Id = _userId
            });

            var sentFriendRequests = _uow.FriendRequestsRepository.Get(f => f.FromUserId == _userId && !f.Resolved.Value).Select(f => new FriendViewModel
            {
                Id = f.ToUserId.Value,
                Username = f.ToUser.Username,
                SentRequest = 1
            }).ToList();
            var receivedFriendRequests = _uow.FriendRequestsRepository.Get(f => f.ToUserId == _userId && !f.Resolved.Value).Select(f => new FriendViewModel
            {
                Id = f.FromUserId.Value,
                Username = f.FromUser.Username,
                SentRequest = 2
            }).ToList();


            var allowedUsers = _uow.UserProfilesRepository.Get().Where(u => (!friends.Select(a => a.Id).Contains(u.Id))
                && !sentFriendRequests.Select(a => a.Id).Contains(u.Id)
                && !receivedFriendRequests.Select(a => a.Id).Contains(u.Id))
                .Select(u => new FriendViewModel
                {
                    Id = u.Id,
                    Username = u.Username,
                    SentRequest = 0
                }).ToList();

            sentFriendRequests.ForEach(r => allowedUsers.Add(r));

            return Ok(allowedUsers);
        }

        [HttpGet]
        [Route("get")]
        public IHttpActionResult GetFriends()
        {
            var friends = _uow.FriendsRepository.Get(f => f.User1.Value == _userId).Select(f => new FriendViewModel
            {
                Id = f.User2.Value,
                Username = f.UserProfile1.Username
            }).ToList();
            return Ok(friends);
        }

        [HttpGet]
        [Route("getFriendsNotInTask/{taskId}")]
        public IHttpActionResult GetFriendsNotInTask(int taskId)
        {
            if (taskId == 0)
            {
                return Ok(_uow.FriendsRepository.Get(f => f.User1.Value == _userId).Select(f => new FriendViewModel
                {
                    Id = f.User2.Value,
                    Username = f.UserProfile1.Username
                }).ToList());
            }

            var task = _uow.TaskRepository.Get(taskId);
            var usersInTask = task.UsersInTasks.Select(u => u.UserId).ToList();
            var friends = _uow.FriendsRepository.Get(f => f.User1.Value == _userId && !usersInTask.Contains(f.User2.Value)).
                Select(f => new FriendViewModel
                {
                    Id = f.User2.Value,
                    Username = f.UserProfile1.Username
                }).ToList();
            return Ok(friends);
        }

        [HttpGet]
        [Route("getFriendsInTask/{taskId}")]
        public IHttpActionResult GetFriendsInTask(int taskId)
        {
            if (taskId == 0)
            {
                return Ok(new List<FriendViewModel>());
            }

            var task = _uow.TaskRepository.Get(taskId);
            var friendsInTask = task.UsersInTasks.Select(u => new FriendViewModel
            {
                Id = u.UserId.Value,
                Username = u.User.Username
            }).ToList();
            return Ok(friendsInTask);
        }

        [HttpDelete]
        [Route("delete")]
        public IHttpActionResult DeleteFriend(int friendId)
        {   
            var friendsRepo = _uow.FriendsRepository;
            var friendship1 = friendsRepo.Get(f => f.User1.Value == _userId && f.User2.Value == friendId).FirstOrDefault();
            var friendship2 = friendsRepo.Get(f => f.User2.Value == _userId && f.User1.Value == friendId).FirstOrDefault();
            var usernameTo = friendship1.UserProfile1.Username;
            
            var myTasks = _uow.TaskRepository.Get(t => t.OwnerId == _userId).ToList();
            foreach(var t in myTasks)
            { 
                var userInTask = _uow.UsersInTasksRepository.Get(ut => ut.TaskId == t.Id && ut.UserId == friendId).FirstOrDefault();
                if (userInTask != null)
                {
                    _uow.UsersInTasksRepository.Remove(userInTask);
                }
            }
            

            friendsRepo.Remove(friendship1);
            friendsRepo.Remove(friendship2);
            _uow.Save();

            NotificationHub.NotifyFriendshipDeleted(usernameTo);
            return Ok();
        }
    }
}
