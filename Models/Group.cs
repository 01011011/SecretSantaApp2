using System;
using System.Collections.Generic;
using System.Threading;

namespace SecretSantaApp.Models
{
    public class Group
    {
        static int nextId;

        public int Id { get; set; }

        Group()
        {
            Id = Interlocked.Increment(ref nextId);
        }
        //public Guid Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }
    }
}