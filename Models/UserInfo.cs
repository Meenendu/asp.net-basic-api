using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDemo.Models
{
    public class UserInfo
    {
        public UserInfo()
        {
            this.Id = Id;
            this.Name = Name;
            this.Email = Email;
            this.Contact = Contact;
            this.City = City;
            this.Address = Address;
            this.UserName = UserName;
            this.Image = Image;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string UserName { get; set; }
        public string Image { get; set; }
    }
}