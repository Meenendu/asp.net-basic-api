using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using ApiDemo.Models;

namespace ApiDemo.Models
{
    public class ApiResponse<T> where T : class
    {
        public ApiResponse(HttpStatusCode status, string message = "")
        {
            this.status = status;
            this.message = message;
        }

        public ApiResponse(HttpStatusCode status, T data, string message = "")
        {
            this.status = status;
            this.message = message;
            if (data != null)
                this.data = new T[] { data };
            else this.data = new T[0];
        }

        public ApiResponse(HttpStatusCode status, IEnumerable<T> data, string message = "")
        {
            this.status = status;
            this.message = message;
            this.data = data;
        }

        public HttpStatusCode status { get; set; }
        public string message { get; set; }
        public IEnumerable<T> data { get; set; }

    }
}