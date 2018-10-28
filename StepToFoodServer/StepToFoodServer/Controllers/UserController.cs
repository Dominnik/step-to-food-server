using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StepToFoodServer.Database;
using StepToFoodServer.Models;
using StepToFoodServer.Repositories;
using StepToFoodServer.Response;

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
                response = new BaseResponse<User>(user);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<User>();
                response.Error = ex.Message;
            }
            return response;
        }

        [HttpPost("register")]
        public BaseResponse<int> Register([FromBody]string name, [FromBody]string login, [FromBody]string password)
        {
            BaseResponse<int> response = null;
            try
            {
                businessLogicLayer.Register(name, login, password);
                response = new BaseResponse<int>(0);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<int> { Error = ex.Message };
            }
            return response;
        }

        [HttpPost("login")]
        public BaseResponse<User> Login([FromBody]string login, [FromBody]string password)
        {
            BaseResponse<User> response = null;
            try
            {
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
        public BaseResponse<User> Check([FromBody]string token)
        {
            BaseResponse<User> response = null;
            try
            {
                User user = businessLogicLayer.Login(token);
                response = new BaseResponse<User>(user);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<User> { Error = ex.Message };
            }
            return response;
        }

        [HttpPost("update/password")]
        public BaseResponse<int> ChangePassword([FromBody]string password, [FromBody]string newPassword)
        {
            BaseResponse<int> response = null;
            try
            {
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
        public BaseResponse<int> Update([FromBody]string name)
        {
            BaseResponse<int> response = null;
            try
            {
                string token = Request.Headers["Auth"];
                User user = userRepository.Filter(elem => elem.Token == token).Single();
                if (user == null)
                    throw new UnauthorizedAccessException("Invalid security token");
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
        public BaseResponse<FileContentResult> GetAvatar()
        {
            BaseResponse<FileContentResult> response = null;
            try
            {
                int userId = int.Parse(Request.Query["userId"]);
                byte[] avatar = Convert.FromBase64String(userRepository.Get(userId).Avatar);
                response = new BaseResponse<FileContentResult>(File(avatar, "image/jpeg"));
            }
            catch (Exception ex)
            {
                response = new BaseResponse<FileContentResult> { Error = ex.Message };
            }
            return response;
        }

        [HttpGet("set/avatar")]
        public BaseResponse<int> SetAvatar()
        {
            BaseResponse<int> response = null;
            try
            {
                string token = Request.Headers["Auth"];
                User user = userRepository.Filter(elem => elem.Token == token).Single();
                if (user == null)
                    throw new UnauthorizedAccessException("Invalid security token");

                string link = Request.Query["uri"];
                using (WebClient webClient = new WebClient())
                {
                    byte[] image = webClient.DownloadData(link);
                    user.Avatar = Convert.ToBase64String(image);
                }
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