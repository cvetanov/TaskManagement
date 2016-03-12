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
    [RoutePrefix("api/friendrequests")]
    public class FriendRequestsController : ApiControllerWithHub<FriendsHub>
    {
        private UnitOfWork _uow;
        private int _userId;

        public FriendRequestsController(IUnitOfWork uow)
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

        [HttpPost]
        [Route("create")]
        public IHttpActionResult SendFriendRequest(FriendViewModel model)
        {
            var friendRequest = new FriendRequest
            {
                FromUserId = _userId,
                ToUserId = model.Id,
                Resolved = false
            };
            _uow.FriendRequestsRepository.Add(friendRequest);
            _uow.Save();

            // send notification to user (if online)
            FriendsHub.NotifyNewFriendRequest(model.Username);

            return Ok();
        }

        [HttpGet]
        [Route("get")]
        public IHttpActionResult GetFriendRequests()
        {
            if (_userId == -1) return Ok(new List<FriendViewModel>());
            var friendRequests = _uow.FriendRequestsRepository.Get(f => f.ToUserId == _userId && !f.Resolved.Value).Select(f => new FriendViewModel
            {
                Id = f.FromUserId.Value,
                Username = f.FromUser.Username
            }).ToList();
            return Ok(friendRequests);
        }

        [HttpPut]
        [Route("reject")]
        public IHttpActionResult RejectFriendRequest(FriendViewModel model)
        {
            var friendRequest = _uow.FriendRequestsRepository.Get(r => r.FromUserId == model.Id && r.ToUserId == _userId && !r.Resolved.Value).FirstOrDefault();
            if (friendRequest == null)
            {
                return NotFound();
            }
            friendRequest.Resolved = true;

            _uow.FriendRequestsRepository.Update(friendRequest);
            _uow.Save();

            FriendsHub.NotifyFriendRequestRejected(friendRequest.FromUser.Username);
            model.Username = friendRequest.FromUser.Username;
            return Ok(model);
        }

        [HttpPut]
        [Route("accept")]
        public IHttpActionResult AcceptFriendRequest(FriendViewModel model)
        {
            var friendRequest = _uow.FriendRequestsRepository.Get(r => r.FromUserId == model.Id && r.ToUserId == _userId && !r.Resolved.Value).FirstOrDefault();
            if (friendRequest == null)
            {
                return NotFound();
            }
            friendRequest.Resolved = true;

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

            FriendsHub.NotifyAccept(friendRequest.FromUser.Username, friendRequest.ToUser.Username);
            model.Username = friendRequest.FromUser.Username;
            return Ok(model);
        }
    }
}
