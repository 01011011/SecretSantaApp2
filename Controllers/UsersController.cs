using SecretSantaApp.Models;
using SecretSantaApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;

namespace SecretSantaApp.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [AllowAnonymous, HttpGet]
        public HttpResponseMessage Get()
        {
            var users = _userRepository.GetAllUsers().ToList();
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        [AllowAnonymous, HttpGet]
        public HttpResponseMessage Get(int id)
        {
            var user = _userRepository.GetUserByGuid(id);

            return user == null 
                ? Request.CreateErrorResponse(HttpStatusCode.NotFound, "There is no user by this guid!") 
                : Request.CreateResponse(HttpStatusCode.OK, user);
        }

        [AllowAnonymous, HttpPost]
        public HttpResponseMessage Post([FromBody]string name)
        {
            var result = _userRepository.SaveUser(new User(name));

            return result
                ? Request.CreateResponse(HttpStatusCode.Created, "User created Sucessfully")
                : Request.CreateErrorResponse(HttpStatusCode.Conflict, "User with the same name already exists!");
        }

        [AllowAnonymous, HttpPut]
        public HttpResponseMessage Put(int id, [FromBody]string name)
        {
            var result = _userRepository.UpdateUser(id, name);

            return result 
                ? Request.CreateResponse(HttpStatusCode.OK, "User updated Sucessfully") 
                : Request.CreateErrorResponse(HttpStatusCode.NotFound, "Could not find any user to update!");
        }

        [AllowAnonymous, HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            var result = _userRepository.DeleteUser(id);

            return result
                ? Request.CreateResponse(HttpStatusCode.OK, "User deleted Sucessfully")
                : Request.CreateErrorResponse(HttpStatusCode.NotFound, "Could not find any user to delete!");
        }

        [AllowAnonymous, HttpGet]
        [ActionName("Match")]
        [Route("api/Users/Match")]
        public HttpResponseMessage Match()
        {
            var result = _userRepository.RunMatchingAlgorithm();

            if (result == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, "Could Not Match!");
            }

            var data = result.Select(u => new
            {
                Sender = u.Key,
                Receiver = u.Value
            });

            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
    }
}
