using Microsoft.AspNet.SignalR.Infrastructure;
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
    public class UsersController : ApiControllerWithHub<NotificationHub>
    {

        private UnitOfWork _uow;
        private int _userId;

        public UsersController(IUnitOfWork uow)
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
    }
}
