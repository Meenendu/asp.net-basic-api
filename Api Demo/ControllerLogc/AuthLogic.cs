using ApiDemo.Auth;
using ApiDemo.Models;
using ApiDemo.Utils;
using System;
using System.Linq;
using System.Net;

namespace ApiDemo.ControllerLogc
{
    public class AuthLogic
    {
        ApiDbContext context = new ApiDbContext();
        public ApiResponse<Token> Login(Login data)
        {
            try
            {

                if (data != null)
                {
                    bool isUsernamePasswordValid = false;
                    using (ApiDbContext context = new ApiDbContext())
                    {
                        var dbUser = context.Logins.Where(x => x.UserName.Equals(data.UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        var md5pass = HashAlgo.MD5Hash(data.Password);
                        isUsernamePasswordValid = dbUser.Password.Equals(md5pass);
                    }

                    // if credentials are valid
                    if (isUsernamePasswordValid)
                    {

                        var user = context.UserTokens.SingleOrDefault(x => x.UserName == data.UserName);
                        if (user != null)
                            context.UserTokens.Remove(user);

                        var token = JwtAuthManager.GenerateJWTToken(data.UserName);
                        UserToken ut = new UserToken()
                        {
                            CreatedBy = data.UserName,
                            ExpireTime = DateTime.UtcNow.AddHours(3),
                            Token = token,
                            CreatedAt = DateTime.UtcNow,
                            TokenType = "LIMITED",
                            UserName = data.UserName
                        };
                        context.UserTokens.Add(ut);
                        context.SaveChanges();
                        Token token1 = new Token()
                        {
                            access_token = token,
                            token_type = "Bearer",
                            expire_time = DateTime.UtcNow.AddHours(3)

                        };
                        return new ApiResponse<Token>(HttpStatusCode.OK, token1);
                    }

                    else
                    {
                        // if credentials are not valid send unauthorized status code in response
                        return new ApiResponse<Token>(HttpStatusCode.Unauthorized, "UnAuthorized Access");
                    }

                }
                else
                {
                    return new ApiResponse<Token>(HttpStatusCode.BadRequest, "Invalid Data");
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<Token>(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        public new ApiResponse<Token> Signup(Login data)
        {
            try
            {
                if (data != null)
                {
                    using (ApiDbContext context = new ApiDbContext())
                    {
                        var dbUser = context.Logins.Where(x => x.UserName.Equals(data.UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        var md5pass = HashAlgo.MD5Hash(data.Password);

                        if (dbUser != null)
                        {
                            return new ApiResponse<Token>(HttpStatusCode.Conflict, "User already exists");
                        }

                        Login login = new Login()
                        {
                            UserName = data.UserName,
                            Password = md5pass
                        };

                        context.Logins.Add(login);
                        context.SaveChanges();
                    }



                    var token = JwtAuthManager.GenerateJWTToken(data.UserName);
                    UserToken ut = new UserToken()
                    {
                        CreatedBy = data.UserName,
                        ExpireTime = DateTime.UtcNow.AddHours(3),
                        Token = token,
                        CreatedAt = DateTime.UtcNow,
                        TokenType = "LIMITED",
                        UserName = data.UserName
                    };
                    context.UserTokens.Add(ut);
                    context.SaveChanges();
                    Token token1 = new Token()
                    {
                        access_token = token,
                        token_type = "Bearer",
                        expire_time = DateTime.UtcNow.AddHours(3)

                    };
                    return new ApiResponse<Token>(HttpStatusCode.OK, token1);
                }

                else
                {
                    return new ApiResponse<Token>(HttpStatusCode.BadRequest, "Invalid Data");
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<Token>(HttpStatusCode.BadRequest, ex.ToString());
            }
        }
    }

}