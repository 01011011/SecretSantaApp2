using System.Collections.Generic;
using System.Web.Http.ModelBinding;
using SecretSantaApp.Infrastructure.ModelBinders;

namespace SecretSantaApp.Models
{
    [ModelBinder(typeof(MatchModelBinder))]
    public class UserGroupViewModel
    {
        public List<Group> Groups { get; set; }
        public List<User> Users { get; set; }
    }
}