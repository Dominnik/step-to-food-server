using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using StepToFoodServer.Database;
using StepToFoodServer.Extensions;
using StepToFoodServer.Models;
using StepToFoodServer.Models.Binds;
using StepToFoodServer.Repositories;
using StepToFoodServer.Response;
using StepToFoodServer.Utils;

namespace StepToFoodServer.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IBusinessLogicLayer businessLogicLayer;
        private readonly IRepository<User> userRepository;

        public UserController(IBusinessLogicLayer businessLogicLayer, IRepository<User> userRepository)
        {
            this.businessLogicLayer = businessLogicLayer;
            this.userRepository = userRepository;
        }

        [HttpGet]
        public BaseResponse<User> Get()
        {
            BaseResponse<User> response = null;
            try
            {
                int userId = int.Parse(Request.Query["userId"]);
                User user = userRepository.Get(userId);
                user.Token = null;
                user.Avatar = ImageLink.GetUserAvatarLink(user.Id);
                response = new BaseResponse<User>(user);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<User> { Error = ex.Message };
            }
            return response;
        }

        [HttpPost("register")]
        public BaseResponse<int> Register(FormDataCollection form)
        {
            BaseResponse<int> response = null;
            try
            {
                string name = form.Name;
                string login = form.Login;
                string password = form.Password;

                int userId = businessLogicLayer.Register(name, login, password);
                response = new BaseResponse<int>(userId);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<int> { Error = ex.Message };
            }
            Login(form);
            return response;
        }

        [HttpPost("login")]
        public BaseResponse<User> Login(FormDataCollection form)
        {
            BaseResponse<User> response = null;
            try
            {
                string login = form.Login;
                string password = form.Password;

                User user = businessLogicLayer.Login(login, password);
                response = new BaseResponse<User>(user);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<User> { Error = ex.Message };
            }
            return response;
        }

        [HttpPost("check")]
        public BaseResponse<User> Check(FormDataCollection form)
        {
            BaseResponse<User> response = null;
            try
            {
                string token = form.Token;
                User user = businessLogicLayer.Check(token);
                response = new BaseResponse<User>(user);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<User> { Error = ex.Message };
            }
            return response;
        }

        [HttpPost("update/password")]
        public BaseResponse<int> ChangePassword(FormDataCollection form)
        {
            BaseResponse<int> response = null;
            try
            {
                string password = form.Password;
                string newPassword = form.NewPassword;
                string token = Request.Headers["Auth"];
                businessLogicLayer.ChangePassword(token, password, newPassword);
                response = new BaseResponse<int>(0);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<int> { Error = ex.Message };
            }
            return response;
        }

        [HttpGet("terminate")]
        public BaseResponse<int> Terminate()
        {
            BaseResponse<int> response = null;
            try
            {
                string token = Request.Headers["Auth"];
                businessLogicLayer.Terminate(token);
                response = new BaseResponse<int>(0);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<int> { Error = ex.Message };
            }
            return response;
        }

        [HttpPost("update/name")]
        public BaseResponse<int> Update(FormDataCollection form)
        {
            BaseResponse<int> response = null;
            try
            {
                string name = form.Name;
                string token = Request.Headers["Auth"];
                User user = businessLogicLayer.Check(token);

                user.Name = name;
                userRepository.Update(user);
                response = new BaseResponse<int>(0);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<int> { Error = ex.Message };
            }
            return response;
        }

        [HttpGet("get/avatar")]
        public ActionResult GetAvatar()
        {
            ActionResult response = null;
            try
            {
                int userId = int.Parse(Request.Query["userId"]);
                string avatarString = userRepository.Get(userId).Avatar;
                byte[] avatar = Convert.FromBase64String(avatarString ?? "");
                response = File(avatar, "image/jpeg");
            }
            catch
            {
                response = new NoContentResult();
            }
            return response;
        }

        [HttpPost("set/avatar")]
        public BaseResponse<int> SetAvatar()
        {
            BaseResponse<int> response = null;
            try
            {
                string token = Request.Headers["Auth"];
                User user = businessLogicLayer.Check(token);

                IFormFile file = Request.Form.Files[0];
                user.Avatar = file.ToBase64String();
                userRepository.Update(user);
                response = new BaseResponse<int>(0);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<int> { Error = ex.Message };
            }
            return response;
        }
    }
}