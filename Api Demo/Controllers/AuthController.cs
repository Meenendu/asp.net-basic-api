using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ApiDemo.Models;
using ApiDemo.ControllerLogc;

namespace ApiDemo.Controllers
{
    public class AuthController : ApiController
    {
        [HttpPost]
        [Route("api/auth/login")]
        public IHttpActionResult Login([FromBody]Login data)
        {
            return Ok(new AuthLogic().Login(data));
        }

        [HttpPost]
        [Route("api/auth/signup")]
        public IHttpActionResult Signin([FromBody]Login data)
        {
            return Ok(new AuthLogic().Signup(data));
        }
    }
}
