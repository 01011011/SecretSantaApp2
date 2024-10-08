using SecretSantaApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecretSantaApp.Infrastructure.Helpers;

namespace SecretSantaApp.Services
{
    public class GroupRepository : IGroupRepository
    {
        private const string CacheKey = "GroupStore";

        public GroupRepository()
        {
            var currentContext = HttpContext.Current;

            if (currentContext == null) return;

            if (currentContext.Cache[CacheKey] == null)
            {
                currentContext.Cache[CacheKey] = new Group[0];
            }
        }
        
        public IEnumerable<Group> GetAllGroups()
        {
            var currentContext = HttpContext.Current;
            return (Group[])currentContext?.Cache[CacheKey];
        }

        public bool SaveGroup(Group group)
        {
            var currentContext = HttpContext.Current;
            if (currentContext == null) return false;

            try
            {
                var groups = ((Group[])currentContext.Cache[CacheKey]).ToList();

                if (GroupExistsByName(groups, group.Name)) return false;

                groups.Add(group);
                currentContext.Cache[CacheKey] = groups.ToArray();
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool RemoveUserFromGroup(int id, User user)
        {
            var currentContext = HttpContext.Current;

            if (currentContext == null) return false;

            try
            {
                var groups = ((Group[])currentContext.Cache[CacheKey]).ToList();

                var group = groups.FirstOrDefault(x => x.Id == id);

                var userToRemove = group?.Users.FirstOrDefault(u => u.Id == user.Id);

                if (userToRemove == null) return false;

                if (group.Users.Count > 2)
                {
                    group.Users.Remove(userToRemove);
                }
                else
                {
                    groups.Remove(group);
                }
                
                currentContext.Cache[CacheKey] = groups.ToArray();
                return true;
                //if this was reached something went wrong.
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool UpdateGroup(int id, List<User> users)
        {
            var currentContext = HttpContext.Current;
            if (currentContext == null) return false;

            try
            {
                var groups = ((Group[])currentContext.Cache[CacheKey]).ToList();
                var group = groups.FirstOrDefault(x => x.Id == id);

                if (group != null)
                {
                    group.Users = group.Users.Union(users, new UserComparer()).ToList();
                }
                currentContext.Cache[CacheKey] = groups.ToArray();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public Group GetGroupById(int id)
        {
            var currentContext = HttpContext.Current;

            var groups = ((Group[]) currentContext?.Cache[CacheKey])?.ToList();

            if (groups != null && groups.Any())
            {
                return groups.FirstOrDefault(x => x.Id == id);
            }

            return null;
        }

        public Group GetGroupByName(string name)
        {
            var currentContext = HttpContext.Current;

            var groups = ((Group[])currentContext?.Cache[CacheKey])?.ToList();

            if (groups != null && groups.Any())
            {
                return groups.FirstOrDefault(x => x.Name == name);
            }

            return null;
        }

        private static bool GroupExistsByName(IEnumerable<Group> groups, string name)
        {
            return groups.Any(x => x.Name == name);
        }

        private static bool GroupExistsById(IEnumerable<Group> groups, int id)
        {
            return groups.Any(x => x.Id == id);
        }
    }
}