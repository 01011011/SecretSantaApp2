using System;
using System.Collections.Generic;
using SecretSantaApp.Models;

namespace SecretSantaApp.Infrastructure.Helpers
{
    public class UserComparer : IEqualityComparer<User>
    {
        public bool Equals(User u1, User u2)
        {
            return u1.Id == u2.Id;
        }

        public int GetHashCode(User p)
        {
            return p.Id;
        }
    }

    public static class RandomizationExtensions
    {
        private static readonly Random Random = new Random();
        // Randomize an array.
        public static void Randomize<T>(this T[] items)
        {
            // For each spot in the array, pick
            // a random item to swap into that spot.
            for (int i = 0; i < items.Length - 1; i++)
            {
                int j = Random.Next(i, items.Length);
                T temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }
        }

        // Randomize a list.
        public static void Randomize<T>(this List<T> items)
        {
            // Convert into an array.
            T[] array = items.ToArray();

            // Randomize.
            array.Randomize();

            // Copy the items back into the list.
            items.Clear();
            items.AddRange(array);
        }
    }
}