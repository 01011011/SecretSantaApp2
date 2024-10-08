using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SecretSantaApp.Models;
using SecretSantaApp.Services;

namespace SecretSantaApp.Controllers
{
    public class GroupsController : ApiController
    {
        private readonly IGroupRepository _groupService;

        public GroupsController(IGroupRepository groupService)
        {
            _groupService = groupService;
        }

        [AllowAnonymous, HttpGet]
        public HttpResponseMessage Get()
        {
            //this is a comment
            var groups = _groupService.GetAllGroups().ToList();
            return Request.CreateResponse(HttpStatusCode.OK, groups);
        }

        [AllowAnonymous, HttpGet]
        public HttpResponseMessage Get(int id)
        {
            var group = _groupService.GetGroupById(id);

            return group == null
                ? Request.CreateErrorResponse(HttpStatusCode.NotFound, "There is no group by this guid!")
                : Request.CreateResponse(HttpStatusCode.OK, group);
        }

        [AllowAnonymous, HttpPost]
        public HttpResponseMessage Post([FromBody]Group group)
        {
            var result = _groupService.SaveGroup(group);

            return result
                ? Request.CreateResponse(HttpStatusCode.Created, "Group created Sucessfully")
                : Request.CreateErrorResponse(HttpStatusCode.Conflict, "Group with the same name already exists!");
        }

        [AllowAnonymous, HttpPut]
        public HttpResponseMessage Put(int id, [FromBody]List<User> users)
        {
            var result = _groupService.UpdateGroup(id, users);

            return result
                ? Request.CreateResponse(HttpStatusCode.OK, "Group updated Sucessfully")
                : Request.CreateErrorResponse(HttpStatusCode.NotFound, "Could not find any group to update!");
        }

        [AllowAnonymous, HttpDelete]
        public HttpResponseMessage Delete(int id, [FromBody]User user)
        {
            var result = _groupService.RemoveUserFromGroup(id, user);

            return result
                ? Request.CreateResponse(HttpStatusCode.OK, "Group deleted Sucessfully")
                : Request.CreateErrorResponse(HttpStatusCode.NotFound, "Could not find any group to delete!");
        }
    }
}
