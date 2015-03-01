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
    class Program {

        ///////////////////////////////////////////////////////////////////////////
        ////////// DOESN'T WORK; HAVE USED ADJACENCY MATRIX ///////////////////////
        //////////////////////////////////////////////////////////////////////////

        static void Main1(string[] args) {
            //first read the data into an adjacency matrix
            string fileName = "kargerMinCut.txt";
            var data =  ReadFile(fileName);
            if (data == null || data.Count == 0) {
                throw new Exception("No data present in graph file.");
            }

            int[,] graph = new int[data.Count, data.Count];
            graph = ZeroGraph(graph);
            graph = FillUpGraph(data, graph);

            while (graph.GetLength(0) > 2) {
                graph = KargerContract(graph);
            }

            Console.WriteLine("Min cut is : {0}", graph[1, 0]);

        }

        private static int[,] KargerContract(int[,] graph) {
            int newDim = graph.GetLength(0)-1;
            int[,] newGraph = new int[newDim, newDim];
            newGraph = ZeroGraph(newGraph);
            
            //pick a random edge u,v uniformly at random.
            int totalEdges = GetTotalEdges(graph);
            Random rand = new Random();
            int luckyEdge = rand.Next(1,totalEdges+1);

            //merge u and v into a single vertex
            newGraph = MergeEdges(graph, luckyEdge);

            //remove self loops
            newGraph = RemoveSelfLoops(newGraph);

            return newGraph;
        }

        private static int[,] RemoveSelfLoops(int[,] graph) {
            int graphDim = graph.GetLength(0);
            for (int i = 0; i < graphDim; i++) {
                graph[i, i] = 0;
            }
            return graph;
        }

        private static int[,] MergeEdges(int[,] graph, int luckyEdge) {
            int graphDim = graph.GetLength(0);
            int newDim = graphDim - 1;
            int[,] newGraph = new int[newDim, newDim];
            newGraph = ZeroGraph(newGraph);

            int u, v;
            Find_u_v(graph, luckyEdge, out u, out v);

            if (u < v) {
                throw new Exception("u is less than v while merging edges.");
            }

            for (int i = 0; i < graphDim; i++) {
                for (int j = 0; j <= i; j++) {
                    int new_i = i;
                    int new_j = j;

                    if (i == u) {
                        new_i = v;
                    }
                    if (j == u) {
                        new_j = v;
                    }

                    if (i > u) {
                        new_i = i - 1;
                    }
                    if (j > u) {
                        new_j = j - 1;
                    }

                    newGraph[new_i, new_j] += graph[i, j];
                }
            }

            return newGraph;
        }

        private static void Find_u_v(int[,] graph, int luckyEdge, out int u, out int v) {
            int graphDim = graph.GetLength(0);
            int currEdgeNum = 0;
            for (int i = 0; i < graphDim; i++) {
                for (int j = 0; j <= i; j++) {
                    currEdgeNum += graph[i, j];
                    if (currEdgeNum >= luckyEdge) {
                        u = i; v = j; return;
                    }
                }
            }
            u = Int32.MinValue; v = Int32.MinValue;
            throw new Exception("Cannot pick a random edge u , v.");
        }

        private static int GetTotalEdges(int[,] graph) {
            int graphDim = graph.GetLength(0);
            int totalEdges = 0;
            for (int i = 0; i < graphDim; i++) {
                for (int j = 0; j <= i; j++) {
                    totalEdges += graph[i, j];
                }
            }
            return totalEdges;
        }

        private static int[,] ZeroGraph(int[,] graph) {
            for (int i = 0; i < graph.GetLength(0); i++) {
                for (int j = 0; j < graph.GetLength(1); j++) {
                    graph[i, j] = 0;
                }
            }
            return graph;
        }

        private static int[,] FillUpGraph(List<string> data, int[,] graph) {
            foreach (string vertexEdges in data) {
                string[] edges_s = vertexEdges.Split(new char[] { '\t' });
                bool fromVertexFound = false;
                int fromVertex = Int32.MinValue;

                foreach (string edge in edges_s) {
                    if (string.IsNullOrWhiteSpace(edge)) continue;
                    if (!fromVertexFound) {
                        int result;
                        if (Int32.TryParse(edge, out result)) {
                            fromVertex = result-1;
                            fromVertexFound = true;
                            continue;
                        }
                    }

                    int incidentEdge;
                    if (Int32.TryParse(edge, out incidentEdge)) {
                        graph[fromVertex, incidentEdge-1] += 1;
                    }
                }
            }
            return graph;
        }

        private static List<string> ReadFile(string fileName) {
            try {
                List<string> data = new List<string>();
                string allData;
                using (StreamReader sr = new StreamReader(fileName)) {
                    allData = sr.ReadToEnd();
                }
                string[] stringRep = allData.Split(new char[] { '\n' });
                foreach (string str in stringRep) {
                    if (String.IsNullOrWhiteSpace(str)) continue;
                    data.Add(str);
                }
                return data;

            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
    }
}
