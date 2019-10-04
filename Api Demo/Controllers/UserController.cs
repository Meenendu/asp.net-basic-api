using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ApiDemo.Models;
using ApiDemo.ControllerLogc;
using ApiDemo.Auth;
using System.Threading.Tasks;
using System.Web;

namespace ApiDemo.Controllers
{
    [JwtAuthentication]
    public class UserController : BaseAPIController
    {
        [HttpGet]
        [Route("api/user")]
        public IHttpActionResult GetAllUsers()
        {
            return Ok(new UserLogic().GetAllUsers(base.getImagePublicUrl("/api/user")));
        }

        [HttpGet]
        [Route("api/user/{id}")]
        public IHttpActionResult GetSingleUser(int id)
        {
            return Ok(new UserLogic().GetSingleUser(base.getImagePublicUrl("/api/user"), id));
        }

        [HttpPost]
        [Route("api/user")]
        public IHttpActionResult StoreUser(User user)
        {
            return Ok(new UserLogic().StoreUser(user));
        }

        [HttpPut]
        [Route("api/user/{id}")]
        public IHttpActionResult UpdateUserDetail(int id, User user)
        {
            return Ok(new UserLogic().UpdateUserDetail(id, user));
        }

        [HttpDelete]
        [Route("api/user/{id}")]
        public IHttpActionResult RemoveUser(int id)
        {
            return Ok(new UserLogic().RemoveUser(id));
        }

        [HttpPost]
        [Route("api/users/{userid}/image")]
        public IHttpActionResult UpdateImage(string userid)
        {
            return Ok(new UserLogic().UpdateImage(userid, HttpContext.Current.Request.Files, User.Identity.Name));

        }
    }
}