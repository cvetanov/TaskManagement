using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using TaskManagement.API.DataLayer.Models;
using TaskManagement.API.ViewModels;
using TaskManagement.Persistence;

namespace TaskManagement.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/Users")]
    public class UsersController : ApiController
    {
        //TODO
        //remove all other attributes from users before sending them to front-end

        private UnitOfWork _uow;
        private int _userId;

        public UsersController(IUnitOfWork uow)
        {
            _uow = uow as UnitOfWork;
            var username = (User.Identity as ClaimsIdentity).Claims.Where(c => c.Type == "sub").FirstOrDefault().Value;
            this._userId = _uow.UserProfilesRepository.Get(u => u.Username == username).First().Id;
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

            var sentFriendRequests = _uow.FriendRequestsRepository.Get(f => f.FromUserId == _userId && !f.Status).Select(f => new FriendViewModel
            {
                Id = f.ToUserId.Value,
                Username = f.ToUser.Username,
                SentRequest = 1
            }).ToList();
            var receivedFriendRequests = _uow.FriendRequestsRepository.Get(f => f.ToUserId == _userId && !f.Status).Select(f => new FriendViewModel
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
        [Route("GetFriends")]
        public IHttpActionResult GetFriends()
        {
            var friends = _uow.FriendsRepository.Get(f => f.User1.Value == _userId).Select(f => new FriendViewModel
            {
                Id = f.User2.Value,
                Username = f.UserProfile1.Username
            }).ToList();
            return Ok(friends);
        }

        [HttpPost]
        [Route("SendFriendRequest")]
        public IHttpActionResult SendFriendRequest(FriendViewModel model)
        {
            var friendRequest = new FriendRequest
            {
                FromUserId = _userId,
                ToUserId = model.Id,
                Status = false
            };
            _uow.FriendRequestsRepository.Add(friendRequest);
            _uow.Save();
            return Ok();
        }

        [HttpGet]
        [Route("GetFriendRequests")]
        public IHttpActionResult GetFriendRequests()
        {
            var friendRequests = _uow.FriendRequestsRepository.Get(f => f.ToUserId == _userId && !f.Status).Select(f => new FriendViewModel
            {
                Id = f.FromUserId.Value,
                Username = f.FromUser.Username
            }).ToList();
            return Ok(friendRequests);
        }

        [HttpPost]
        [Route("RejectFriendRequest")]
        public IHttpActionResult RejectFriendRequest(FriendViewModel model)
        {
            var friendRequest = _uow.FriendRequestsRepository.Get(r => r.FromUserId == model.Id && r.ToUserId == _userId).FirstOrDefault();
            if (friendRequest == null)
            {
                return NotFound();
            }
            friendRequest.Status = true;

            _uow.FriendRequestsRepository.Update(friendRequest);
            _uow.Save();

            model.Username = friendRequest.FromUser.Username;
            return Ok(model);
        }

        [HttpPost]
        [Route("AcceptFriendRequest")]
        public IHttpActionResult AcceptFriendRequest(FriendViewModel model)
        {
            var friendRequest = _uow.FriendRequestsRepository.Get(r => r.FromUserId == model.Id && r.ToUserId == _userId).FirstOrDefault();
            if (friendRequest == null)
            {
                return NotFound();
            }
            friendRequest.Status = true;

            var friendship1 = new Friendship
            {
                DateStart = DateTime.Now,
                User1 = friendRequest.FromUserId,
                User2 = friendRequest.ToUserId
            };
            var friendship2 = new Friendship
            {
                DateStart = DateTime.Now,
                User1 = friendRequest.ToUserId,
                User2 = friendRequest.FromUserId
            };

            _uow.FriendRequestsRepository.Update(friendRequest);
            _uow.FriendsRepository.Add(friendship1);
            _uow.FriendsRepository.Add(friendship2);
            _uow.Save();

            model.Username = friendRequest.FromUser.Username;
            return Ok(model);
        }
    }
}
