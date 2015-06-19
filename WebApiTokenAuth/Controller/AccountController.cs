using Microsoft.AspNet.Identity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiTokenAuth.Model;
using WebApiTokenAuth.DataAccess;
using WebApiTokenAuth.Utility;
using System.Collections.Generic;

namespace WebApiTokenAuth.Controller
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        DataAccessLayer _repo = null;

        public AccountController()
        {
            _repo = new DataAccessLayer();
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register"),HttpPost]
        public HttpResponseMessage Register(UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse<System.Web.Http.ModelBinding.ModelStateDictionary>(HttpStatusCode.BadRequest, ModelState);
              //  return BadRequest(ModelState);   
            }

            AppResultModel result= _repo.RegisterUser(userModel);
            if (result.ResultStatus!=(int)AppResultStatus.SUCCESS)
            {
              //  return BadRequest(result.ResultMessage);
                return Request.CreateResponse<AppResultModel>(HttpStatusCode.Conflict, result);
            }
            else
            {
               // return Ok(result.ResultMessage);
               return Request.CreateResponse<AppResultModel>(HttpStatusCode.OK, result);
            }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        [AllowAnonymous]
        [ActionName("GetUserInfo"), HttpGet]
        public IHttpActionResult GetUserInfo(string mobileNo)
        {
           var user= _repo.GetUser(mobileNo);
           if (user!=null)
           {
               return Ok(user);
           }
           else
           {
               return BadRequest();
           }
        }


    }
}
