using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TaskManagement.API.Controllers
{
    [RoutePrefix("api/userProfile")]
    [Authorize]
    public class UserProfilesController : ApiController
    {
        //TODO: show some statistics about this user
    }
}
