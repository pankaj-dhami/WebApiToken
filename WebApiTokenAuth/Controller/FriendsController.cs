using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiTokenAuth.DataAccess;
using WebApiTokenAuth.Model;

namespace WebApiTokenAuth.Controller
{
    public class FriendsController : ApiController
    {
        DataAccessLayer _repo = null;
        public FriendsController()
        {
            _repo = new DataAccessLayer();
        }
        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult POST(int userID, [FromBody] List<TelephoneNumberModel> friendsUserModel)
        {
            //List<UserModel> friendsUserModel = new List<UserModel>();
            List<UserModel> result = _repo.RegisterFriends(friendsUserModel, userID);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult GET(int userID)
        {
            //List<UserModel> friendsUserModel = new List<UserModel>();
            List<UserModel> result = _repo.GetUpdatedFriendsList(userID);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
