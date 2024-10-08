using System;
using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;
using SecretSantaApp.Models;

namespace SecretSantaApp.Infrastructure.ModelBinders
{
    public class MatchModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var model = (UserGroupViewModel)bindingContext.Model ?? new UserGroupViewModel();

            var groups = bindingContext.ValueProvider.GetValue("groups");
            if (groups == null)
            {
                throw new Exception("Missing Parameter: " + "groups");
            }
            model.Groups = JsonConvert.DeserializeObject<List<Group>>(groups.AttemptedValue);

            var users = bindingContext.ValueProvider.GetValue("users");
            if (users == null)
            {
                throw new Exception("Missing Parameter: " + "users");
            }
            model.Users = JsonConvert.DeserializeObject<List<User>>(users.AttemptedValue);

            bindingContext.Model = model;
            return true;
        }

    }
}