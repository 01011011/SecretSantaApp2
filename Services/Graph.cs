using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Ajax.Utilities;
using SecretSantaApp.Infrastructure.Helpers;
using SecretSantaApp.Models;

namespace SecretSantaApp.Services
{
    public class Graph
    {
        private Dictionary<string,string> matches = new Dictionary<string, string>();
        public List<string> Vertices { get; set; }
        public Dictionary<string, List<string>> AdjacencyList { get; } = new Dictionary<string, List<string>>();
        public Graph() { }
        public Graph(List<string> users, IEnumerable<Tuple<string, string>> edges)
        {
            Vertices = users;

            foreach (var user in users)
            {
                AddVertex(user);
            }

            foreach (var edge in edges)
            {
                AddEdge(edge);
            }
        }

        public void AddVertex(string vertex)
        {
            AdjacencyList[vertex] = new List<string>();
        }

        public void AddEdge(Tuple<string, string> edge)
        {
            if (AdjacencyList.ContainsKey(edge.Item1) && AdjacencyList.ContainsKey(edge.Item2))
            {
                AdjacencyList[edge.Item1].Add(edge.Item2);
            }
        }

        bool Check(List<string> possibleEdges, string gifter,
            Dictionary<string, bool> visited, Dictionary<string, bool> receivers)
        {
            for (int v = 0; v < receivers.Count; v++)
            {
                var receiver = receivers.Keys.ElementAt(v);
                if (possibleEdges.Contains(receiver)
                    && !visited.ContainsKey(receiver))
                {
                    visited.Add(receiver, true);

                    if (receivers.Values.ElementAt(v) || Check(possibleEdges, gifter,
                            visited, receivers))
                    {
                        matches.Add(gifter,receiver);
                        receivers.Remove(receiver);
                        return true;
                    }
                }
            }
            return false;
        }

        public Dictionary<string, string> GetMatchedData()
        {
            var randomReceivers = new List<string>();
            Vertices.CopyItemsTo(randomReceivers);
            randomReceivers.Randomize();

            var receivers = new Dictionary<string, bool>();
            foreach (var vertex in randomReceivers)
            {
                receivers.Add(vertex, true);
            }

            var users = Vertices.Select(x => new {Name = x,
                 AdjacencyList[x].Count})
                .OrderBy(a => a.Count)
                .Select(b => b.Name);

            // Count of matchings
            int result = 0;
            foreach (var gifter in users)
            {
                var visited = new Dictionary<string, bool>();

                //Find if user can be matched
                if (Check(AdjacencyList[gifter], gifter, visited, receivers))
                {
                    result++;
                }
                    
            }

            return result == Vertices.Count ? matches : new Dictionary<string, string>();
        }
    }
}