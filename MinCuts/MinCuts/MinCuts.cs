using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MinCuts {
    /// <summary>
    /// RandomContractionAlgorithm(Graph G)
    ///     While there are more than 2 vertices
    ///	        pick a remaining edge (u,v) uniformly at random
    ///	        merge u and v into a single vertex
    ///	        remove self loops
    ///     Return cut represented by final 2 vertices
    ///
    /// Run the algorithm several times and choose the minimum min-cut.
    /// </summary>
    class MinCuts {
        static Dictionary<int, List<int>> staticGraph = new Dictionary<int, List<int>>();
        static Dictionary<int, List<int>> graph = new Dictionary<int, List<int>>();
        static int LAST_NODE = Int32.MinValue;

        static void Main(string[] args) {
            string inputFile = "KargerMinCut.txt";
            int currentAttempt = 0;
            int MAX_ATTEMPT = 200 * 200;
            int RUNNING_MIN_CUT_VALUE = Int32.MaxValue;

            Console.WriteLine("Starting the program.");
            ParseGraph(inputFile);

            while (currentAttempt++ < MAX_ATTEMPT) {
                graph = new Dictionary<int, List<int>>();
                CloneGraph(staticGraph, graph);

                //Console.WriteLine("Contracting run {0}", currentAttempt);

                while (graph.Count > 2) {
                    //Console.WriteLine("Graph node count is {0}", graph.Count);
                    int u, v;
                    PickRandomEdge(graph, out u, out v);
                    ContractGraph(u, v);
                }

                //Console.WriteLine("Contracted the graph.");
                Console.WriteLine("Number of min-cut edges in attempt {0} is {1}", currentAttempt, graph.ElementAt(0).Value.Count);

                if (graph.ElementAt(0).Value.Count < RUNNING_MIN_CUT_VALUE) {
                    RUNNING_MIN_CUT_VALUE = graph.ElementAt(0).Value.Count;
                }
            }

            Console.WriteLine("====== FINAL MIN CUT VALUE IS {0} =========", RUNNING_MIN_CUT_VALUE);

            Console.ReadLine();

        }

        private static void CloneGraph(Dictionary<int, List<int>> source, Dictionary<int, List<int>> destination) {
            foreach (var connectionDetail in source) {
                destination.Add(connectionDetail.Key, new List<int>());
                foreach (int dest in connectionDetail.Value) {
                    destination[connectionDetail.Key].Add(dest);
                }
            }
        }

        private static void ContractGraph(int u, int v) {
            LAST_NODE++;
            graph.Add(LAST_NODE, new List<int>());

            List<int> eltsToCheck = new List<int>();
            eltsToCheck.AddRange(graph[u]);
            eltsToCheck.AddRange(graph[v]);

            foreach (int incidentElt in eltsToCheck) {
                if (incidentElt == u || incidentElt == v) continue;
                graph[LAST_NODE].Add(incidentElt);
            }

            if (!graph.Remove(u)) throw new InvalidOperationException(String.Format("Element {0} not present in the graph.", u));
            if (!graph.Remove(v)) throw new InvalidOperationException(String.Format("Element {0} not present in the graph.", v));

            foreach (var nodeConnection in graph) {
                if (nodeConnection.Key == LAST_NODE) continue;
                int count = nodeConnection.Value.RemoveAll(x => x == u || x == v);
                if (count > 0) {
                    nodeConnection.Value.AddRange(Enumerable.Range(0, count).Select(_ => LAST_NODE));
                }
            }
        }

        private static void PickRandomEdge(Dictionary<int, List<int>> graph, out int u, out int v) {
            Random rand = new Random();
            int r1 = rand.Next(graph.Count);
            
            var elt = graph.ElementAt(r1);
            int r2 = rand.Next(elt.Value.Count);

            u = elt.Key;
            v = elt.Value.ElementAt(r2);
        }

        private static void ParseGraph(string inputFile) {
            using (StreamReader sr = new StreamReader(inputFile)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    int fromEdge;
                    List<int> incidentEdges = ReadEdges(line, out fromEdge);
                    if (!staticGraph.ContainsKey(fromEdge)) {
                        staticGraph.Add(fromEdge, incidentEdges);
                    } else {
                        staticGraph[fromEdge].AddRange(incidentEdges);
                    }

                    if (fromEdge > LAST_NODE) LAST_NODE = fromEdge;
                }
            }
        }

        private static List<int> ReadEdges(string line, out int fromEdge) {
            fromEdge = Int32.MinValue;
            
            if (line == null || line.Trim() == string.Empty) {
                return null;
            }

            string[] nodes = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (nodes == null || nodes.Length == 0) {
                return null;
            }

            List<int> intList = nodes.Select(x => Int32.Parse(x)).ToList();

            fromEdge = intList[0];
            intList.RemoveAt(0);
            return intList;

        }
    }
}
