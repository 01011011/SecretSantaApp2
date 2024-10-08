using SecretSantaApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SecretSantaApp.Services
{
    public class UserRepository : IUserRepository
    {
        private const string CacheKey = "UserStore";

        public UserRepository()
        {
            var currentContext = HttpContext.Current;

            if (currentContext == null) return;

            if (currentContext.Cache[CacheKey] == null)
            {
                currentContext.Cache[CacheKey] = new User[0];
            }
        }

        public IEnumerable<User> GetAllUsers()
        {
            var currentContext = HttpContext.Current;
            return (User[]) currentContext?.Cache[CacheKey];
        }

        public bool SaveUser(User user)
        {
            var currentContext = HttpContext.Current;
            if (currentContext == null) return false;

            try
            {
                var users = ((User[])currentContext.Cache[CacheKey]).ToList();

                if (UserExistsByName(users, user.Name)) return false;

                users.Add(user);
                currentContext.Cache[CacheKey] = users.ToArray();
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool DeleteUser(int id)
        {
            var currentContext = HttpContext.Current;

            if (currentContext == null) return false;

            try
            {
                var users = ((User[])currentContext.Cache[CacheKey]).ToList();

                if (!UserExistsByGuid(users, id)) return false;
                var userToDelete = GetUserByGuid(id);
                //remove user from userlist
                var result = users.Remove(userToDelete);

                currentContext.Cache[CacheKey] = users.ToArray();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool UpdateUser(int id, string name)
        {
            var currentContext = HttpContext.Current;

            if (currentContext == null) return false;

            try
            {
                var users = ((User[])currentContext.Cache[CacheKey]).ToList();

                if (!UserExistsByGuid(users, id)) return false;

                //if we got here there is user, no need to check for null ref
                var user = users.First(x => x.Id == id);
                user.Name = name;

                currentContext.Cache[CacheKey] = users.ToArray();
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public User GetUserByGuid(int id)
        {
            var currentContext = HttpContext.Current;

            var users = ((User[]) currentContext?.Cache[CacheKey])?.ToList();

            if (users != null && users.Any())
            {
                return users.FirstOrDefault(x => x.Id == id);
            }

            return null;
        }

        public Dictionary<string, string> RunMatchingAlgorithm()
        {
            //lets do the fun work :)
            var currentContext = HttpContext.Current;

            var users = ((User[])currentContext?.Cache[CacheKey])?.ToList();
            var groups = ((Group[])currentContext?.Cache["GroupStore"])?.ToList();

            //we should have everything by now, just doing another check.
            if (users == null || groups == null) return null;

            //set all possible undirected edges between verticles (users)
            var edges = new List<Tuple<string, string>>();
            foreach(var user in users)
            {
                var group = groups.FirstOrDefault(x => x.Users.Any(m => m.Id == user.Id));

                if (group != null)
                {
                    foreach (var user2 in users)
                    {
                        if (group.Users.All(c => c.Id != user2.Id))
                        {
                            edges.Add(new Tuple<string, string>(user.Name, user2.Name));
                        }
                    }
                }
                else
                {
                    foreach (var user2 in users)
                    {
                        if (user.Id != user2.Id)
                        {
                            edges.Add(new Tuple<string, string>(user.Name, user2.Name));
                        }
                    }
                }
            }

            var stringUsers = users.Select(x => x.Name).ToList();
            var graph = new Graph(stringUsers, edges);
            return graph.GetMatchedData();
        }

        private bool UserExistsByName(IEnumerable<User> users, string name)
        {
            return users.Any(x => x.Name.ToLowerInvariant() == name.ToLowerInvariant());
        }

        private bool UserExistsByGuid(IEnumerable<User> users, int id)
        {
            return users.Any(x => x.Id == id);
        }
    }
}

