using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using TaskManagement.API.ViewModels;
using TaskManagement.Persistence;

namespace TaskManagement.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/userProfile")]
    public class UserProfilesController : ApiController
    {
        private UnitOfWork _uow;
        private int _userId;
        public UserProfilesController(IUnitOfWork uow)
        {
            _uow = uow as UnitOfWork;
            var username = User.Identity.Name;
            _userId = _uow.UserProfilesRepository.Get(u => u.Username == username).First().Id;
        }
        
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            if (id == 0)
                id = _userId;

            var user = _uow.UserProfilesRepository.Get(id);
            var friendship = _uow.FriendsRepository.Get(f => f.User1.Value == _userId && f.User2.Value == id).FirstOrDefault();
            var isFriend = friendship != null;
            var friendRequestSent = _uow.FriendRequestsRepository.Get(r => r.FromUserId == _userId && r.ToUserId == id && r.Resolved.HasValue && !r.Resolved.Value).FirstOrDefault();
            var friendRequestReceived = _uow.FriendRequestsRepository.Get(r => r.ToUserId == _userId && r.FromUserId == id && r.Resolved.HasValue && !r.Resolved.Value).FirstOrDefault();
            var viewModel = new UserViewModel
            {
                UserId = user.Id,
                Username = user.Username,
                CreatedTasks = user.OwnedTasks.Count,
                OtherTasks = user.SharedTasks.Count,
                FinishedOwnTasks = user.OwnedTasks.Where(t => t.Status.HasValue && t.Status.Value).Count(),
                FinishedOtherTasks = user.SharedTasks.Where(t => t.Task.Status.HasValue && t.Task.Status.Value).Count(),
                Friends = user.Friendships.Count,
                Comments = _uow.CommentsRepository.Get(c => c.UserId == user.Id).Count(),
                IsFriend = isFriend,
                FriendRequestSent = friendRequestSent != null,
                FriendRequestReceived = friendRequestReceived != null,
                Image = user.ProfilePhotoUrl
            };
            if (isFriend)
            {
                viewModel.FriendSince = friendship.DateStart.Value;
            }
            if (_userId == id)
            {
                viewModel.Me = true;
            }
            else
            {
                viewModel.Me = false;
            }
            return Ok(viewModel);
        }
    }
}
