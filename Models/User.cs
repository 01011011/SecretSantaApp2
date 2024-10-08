using SecretSantaApp.Services;
using System.Threading;

namespace SecretSantaApp.Models
{
    public class User
    {
        static int nextId;
        public int Id { get; set; }

        public User(string name)
        {
            Id = Interlocked.Increment(ref nextId);
            Name = name;
        }

        public string Name { get; set; }

        public bool InGroup { get; set; }
    }
}