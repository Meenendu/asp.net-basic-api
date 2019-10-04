using ApiDemo.Auth;
using ApiDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace ApiDemo.Controllers
{
    public class BaseAPIController : ApiController
    {

        protected string getImagePublicUrl(string actions_segments)
        {
            string imageUrlHost = Request.RequestUri.AbsoluteUri.Substring(0, Request.RequestUri.AbsoluteUri.IndexOf(actions_segments));
            return imageUrlHost; 
        }
        
    }
}
