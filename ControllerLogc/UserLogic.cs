using ApiDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace ApiDemo.ControllerLogc
{
    public class UserLogic
    {
        public ApiResponse<UserInfo> GetAllUsers(string imagePath)
        {
            using (ApiDbContext dbContext = new ApiDbContext())
            {
                List<UserInfo> data = new List<UserInfo>();
                var Users = dbContext.Users.ToList();
                foreach (var x in Users)
                {
                    UserInfo obj = new UserInfo();
                    obj.Name = x.Name;
                    obj.Contact = x.Contact;
                    obj.Email = x.Email;
                    obj.City = x.City;
                    obj.Address = x.Address;
                    obj.Id = x.Id;
                    obj.Image = Utils.Utils.ImageURL(imagePath, x.Image);
                    data.Add(obj);
                }
                //var Users = (from e in dbContext.Users
                //            join d in dbContext.Logins on e.UserName equals d.UserName
                //            select new UserInfo()
                //            {
                //                UserName = e.Name,
                //                Name = e.UserName,
                //                Email = d.Password
                //            }).ToList();

                return new ApiResponse<UserInfo>(HttpStatusCode.OK, data, "");
            }
        }

        public ApiResponse<UserInfo> GetSingleUser(string imagePath, int id)
        {
            using (ApiDbContext dbContext = new ApiDbContext())
            {
                var User = dbContext.Users.Where(x => x.Id == id).FirstOrDefault();
                if (User == null)
                    return new ApiResponse<UserInfo>(HttpStatusCode.NotFound, "User Not Found");

                UserInfo data = new UserInfo();
                data.Name = User.Name;
                data.Contact = User.Contact;
                data.Email = User.Email;
                data.City = User.City;
                data.Address = User.Address;
                data.Id = User.Id;
                data.Image = Utils.Utils.ImageURL(imagePath, User.Image);

                return new ApiResponse<UserInfo>(HttpStatusCode.OK, data, "");
            }
        }

        public ApiResponse<User> StoreUser(User data)
        {
            using (ApiDbContext dbContext = new ApiDbContext())
            {
                dbContext.Users.Add(data);
                dbContext.SaveChanges();
                return new ApiResponse<User>(HttpStatusCode.Created, "User Created Successfully");
            }
        }

        public ApiResponse<User> UpdateUserDetail(int id, User data)
        {
            using (ApiDbContext dbContext = new ApiDbContext())
            {
                var user = dbContext.Users.SingleOrDefault(x => x.Id == id);
                if (user == null)
                    return new ApiResponse<User>(HttpStatusCode.NotFound, "User Not Found");

                user.Contact = data.Contact;
                user.Name = data.Name;
                user.City = data.City;
                user.Address = data.Address;

                dbContext.SaveChanges();
                return new ApiResponse<User>(HttpStatusCode.OK, user, "Success");
            }
        }

        public ApiResponse<User> RemoveUser(int id)
        {
            using (ApiDbContext dbContext = new ApiDbContext())
            {
                var user = dbContext.Users.SingleOrDefault(x => x.Id == id);
                if (user == null)
                    return new ApiResponse<User>(HttpStatusCode.NotFound, "User Not Found");

                dbContext.Users.Remove(user);
                dbContext.SaveChanges();
                return new ApiResponse<User>(HttpStatusCode.OK, "Record Deleted Successfully");
            }
        }

        public ApiResponse<UserInfo> UpdateImage(string forUser, HttpFileCollection files, string username)
        {
            ApiDbContext context = new ApiDbContext();
            try
            {
                var u = context.Users.Where(x => x.UserName == username).FirstOrDefault();
                if (u == null)
                    return new ApiResponse<UserInfo>(HttpStatusCode.NotFound, "invalid data. user does not exist");

                foreach (string file in files)
                {
                    var postedFile = files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            var message = string.Format("Please Upload image of type .jpg,.gif,.png.");
                            return new ApiResponse<UserInfo>(HttpStatusCode.Conflict, message);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {
                            var message = string.Format("Please Upload a file upto 1 mb.");
                            return new ApiResponse<UserInfo>(HttpStatusCode.Conflict, message);
                        }
                        else
                        {
                            var fileName = $"{forUser}{postedFile.FileName.Substring(postedFile.FileName.Length - 4)}";
                            var filePath = HttpContext.Current.Server.MapPath("~/"); // profile_images/" + postedFile.FileName + extension);
                            filePath = Utils.Utils.ImageURL(filePath, fileName);
                            postedFile.SaveAs(filePath);

                            System.IO.File.Copy(filePath, filePath.Insert(filePath.Length - 4, "_small"), true);

                            u.Image = fileName;
                            context.SaveChanges();

                            return new ApiResponse<UserInfo>(HttpStatusCode.OK, "Image Updated Successfully.");
                        }
                    }
                }

                var res = string.Format("Please Upload a image.");
                return new ApiResponse<UserInfo>(HttpStatusCode.BadRequest, res);
            }
            catch (Exception ex)
            {
                var res = string.Format(ex.Message);
                return new ApiResponse<UserInfo>(HttpStatusCode.BadRequest, ex.ToString());
            }
        }
    }
}