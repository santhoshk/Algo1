using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace ShortestPath {
    class Edge{
        public int Vertex { get; set; }
        public int Weight { get; set; }
    }


    class Program {

        static Dictionary<int, List<Edge>> graph = new Dictionary<int, List<Edge>>();        

        static void Main(string[] args) {
            string fileName = "dijkstraData.txt";

            Console.WriteLine("Starting the program.");
            ParseGraph(fileName);
            Console.WriteLine("Finished Parsing the graph.");

            int sourceVertex = 1;

            Console.WriteLine("Calculating the shortest paths from the source vertex {0}", sourceVertex);
            var shortestDistances = DijkstraShortestPath(graph, sourceVertex);
            Console.WriteLine("Finished calculating the shortest paths from the source vertex.");

            int[] testTargetVertices = new int[] { 7, 37, 59, 82, 99, 115, 133, 165, 188, 197 };
            for (int i = 0; i < testTargetVertices.Length; i++) {
                Console.Write(shortestDistances[testTargetVertices[i]] + ",");
            }
        }

        private static Dictionary<int, int> DijkstraShortestPath(Dictionary<int, List<Edge>> graph, int sourceVertex) {
            Dictionary<int, int> shortestDistances = new Dictionary<int, int>();
            shortestDistances.Add(sourceVertex, 0);

            HashSet<int> discoveredVertices = new HashSet<int>();
            discoveredVertices.Add(sourceVertex);

            HashSet<int> unDiscoveredVertices = new HashSet<int>();
            foreach (int vertex in graph.Keys) {
                if (vertex != sourceVertex) unDiscoveredVertices.Add(vertex);
            }

            //for each vertex in foundVertices, get the smallest emanating edge to the undiscovered vertices
            //and add it to the list of discovered vertices
            while (unDiscoveredVertices.Count > 0) {
                int minWeight = Int32.MaxValue;
                int luckyNode = -1;
                foreach (int vertex in discoveredVertices) {
                    var edges = graph[vertex];
                    foreach (var edge in edges) {
                        if (
                            (unDiscoveredVertices.Contains(edge.Vertex)) &&
                            (shortestDistances[vertex] + edge.Weight < minWeight) 
                        ) {
                            minWeight = shortestDistances[vertex] + edge.Weight;
                            luckyNode = edge.Vertex;
                        }
                    }
                }

                discoveredVertices.Add(luckyNode);
                unDiscoveredVertices.Remove(luckyNode);
                shortestDistances[luckyNode] = minWeight;
            }

            return shortestDistances;
        }

        private static void ParseGraph(string fileName) {
            using (StreamReader sr = new StreamReader(fileName)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    string[] data = line.Split(new char[] { '\t', ' ' });
                    if (data == null || data.Length < 2) {
                        throw new FormatException(String.Format("Current line not in valid format {0}", line));
                    }
                    string vertex_s = data[0];
                    int sourceVertex;
                    if (!Int32.TryParse(vertex_s, out sourceVertex)) {
                        throw new InvalidCastException(String.Format("Invalid cast while casting source vertex in the line {0}", line));
                    }

                    List<Edge> nodes = new List<Edge>();

                    for (int i = 1; i < data.Length; i++) {
                        if (string.IsNullOrWhiteSpace(data[i])) continue;
                        string[] pair = data[i].Split(new char[] { ',' });
                        if (pair == null || pair.Length != 2) {
                            throw new FormatException(String.Format("Current pair is not in valid format {0}", data[i]));
                        }

                        int vertex, weight;
                        if (!Int32.TryParse(pair[0], out vertex)) {
                            throw new InvalidCastException(String.Format("Invalid cast while casting vertex in the pair {0}", data[i]));
                        }
                        if (!Int32.TryParse(pair[1], out weight)) {
                            throw new InvalidCastException(String.Format("Invalid cast while casting weight in the pair {0}", data[i]));
                        }

                        nodes.Add(new Edge() { Vertex = vertex, Weight = weight });
                    }
                    graph.Add(sourceVertex, nodes);
                }
            }
        }
    }
}
