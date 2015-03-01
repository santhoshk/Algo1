using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading;

namespace ConnectedComponents {

    class Node {
        public int Value {get;set;}
        public bool Explored {get;set;}
        public int Leader { get; set; }
        public int FinishTime { get; set; }
    }

    class Program {
        const int TOTAL_NODES = 875714;
        static Dictionary<int, Node> graphNodes = new Dictionary<int, Node>(TOTAL_NODES);
        static Dictionary<int, List<int>> graphOrig = new Dictionary<int, List<int>>(TOTAL_NODES);
        static Dictionary<int, List<int>> graphRev = new Dictionary<int, List<int>>(TOTAL_NODES);
        static Dictionary<int, int> finishTimeToNode = new Dictionary<int, int>();
        static Dictionary<int, int> leaderToComponentSize = new Dictionary<int, int>();

        static int FINISH_TIME = 0;
        static int MAX_FINISH_TIME = Int32.MinValue;
        static int CURRENT_SOURCE = Int32.MinValue;


        static void Main(string[] args) {
            string inputFile = "SCC.txt";

            InitializeGraphs();

            ParseGraph(inputFile);

            Console.WriteLine("Parsed the graph into a adjacency list.");

            ReverseGraph();

            Console.WriteLine("Reversed the graph.");

            Console.WriteLine("Starting 1st pass.");

            Thread t1 = new Thread(new ThreadStart(DFSLoop1), 8 * 1024 * 1024);
            t1.Start();
            t1.Join();

            Console.WriteLine("Finished 1st pass.");

            SetAllNodesAsUnExplored();

            Console.WriteLine("Starting 2nd pass..");

            Thread t2 = new Thread(new ThreadStart(DFSLoop2), 16 * 1024 * 1024);
            t2.Start();
            t2.Join();

            Console.WriteLine("Finished 2nd pass..");

            Console.WriteLine("Calculating the strongly connected component sizes.");

            CalculateComponentSizes();

            Console.WriteLine("The top 5 component sizes are.");

            var v = from entry in leaderToComponentSize orderby entry.Value descending select entry.Value;
            var componentSizes = v.ToArray();

            Console.WriteLine("{0},{1},{2},{3},{4}", componentSizes[0], componentSizes[1], componentSizes[2], componentSizes[3], componentSizes[4]);

            Console.ReadLine();
        }

        private static void CalculateComponentSizes() {
            foreach (var nodeDetail in graphNodes) {
                if (!leaderToComponentSize.ContainsKey(nodeDetail.Value.Leader)) {
                    leaderToComponentSize.Add(nodeDetail.Value.Leader, 0);
                }
                leaderToComponentSize[nodeDetail.Value.Leader]++;
            }
        }

        private static void SetAllNodesAsUnExplored() {
            for (int i = 1; i <= TOTAL_NODES; i++) {
                graphNodes[i].Explored = false;
                graphNodes[i].Leader = Int32.MinValue;
            }
        }

        private static void InitializeGraphs() {
            for (int i = 1; i <= TOTAL_NODES; i++) {
                graphNodes.Add(i, new Node() {Value = i, Explored = false });
                graphOrig.Add(i, new List<int>());
                graphRev.Add(i, new List<int>());
            }
        }

        private static void DFSLoop1() {
            FINISH_TIME = 0;
            CURRENT_SOURCE = Int32.MinValue;
            for (int i = TOTAL_NODES; i > 0; i--) {
                if (!graphNodes[i].Explored) {
                    CURRENT_SOURCE = i;
                    DFS1(i);
                }
            }
        }

        private static void DFSLoop2() {
            FINISH_TIME = 0;
            CURRENT_SOURCE = Int32.MinValue;
            for (int i = MAX_FINISH_TIME; i > 0; i--) {
                int currNode = finishTimeToNode[i];
                if (!graphNodes[currNode].Explored) {
                    CURRENT_SOURCE = currNode;
                    DFS2(currNode);
                }
            }
        }

        private static void DFS1(int node) {
            graphNodes[node].Explored = true;
            graphNodes[node].Leader = CURRENT_SOURCE;
            foreach (int incidentNode in graphRev[node]) {
                if (!graphNodes[incidentNode].Explored) {
                    DFS1(incidentNode);
                }
            }
            graphNodes[node].FinishTime = ++FINISH_TIME;
            finishTimeToNode.Add(FINISH_TIME, node);
            if (MAX_FINISH_TIME < FINISH_TIME) {
                MAX_FINISH_TIME = FINISH_TIME;
            }
        }

        private static void DFS2(int node) {
            graphNodes[node].Explored = true;
            graphNodes[node].Leader = CURRENT_SOURCE;
            foreach (int incidentNode in graphOrig[node]) {
                if (!graphNodes[incidentNode].Explored) {
                    DFS2(incidentNode);
                }
            }
            //graphNodes[node].FinishTime = ++FINISH_TIME;
            //finishTimeToNode.Add(FINISH_TIME, node);
            //if (MAX_FINISH_TIME < FINISH_TIME) {
            //    MAX_FINISH_TIME = FINISH_TIME;
            //}
        }

        private static void ParseGraph(string inputFile) {
            using (StreamReader sr = new StreamReader(inputFile)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    int from, to;
                    if (ReadEdges(line, out from, out to)) {
                        if (!graphOrig.ContainsKey(from)) {
                            throw new ArgumentOutOfRangeException("Node " + from + " is outside of allowed range.");
                        }
                        if (graphOrig[from].Contains(to)) {
                            throw new InvalidOperationException(String.Format(
                                "To edge {0} already exists for the from edge {1}", to, from));
                        }

                        graphOrig[from].Add(to);
                    } else {
                        Console.WriteLine(
                            "Warning! line {0} is not a valid line in the input file.", line);
                    }
                }
            }
        }

        private static void ReverseGraph() {
            foreach (var nodeConnections in graphOrig) {
                foreach (int toEdge in nodeConnections.Value) {
                    if (!graphRev[toEdge].Contains(nodeConnections.Key)) {
                        graphRev[toEdge].Add(nodeConnections.Key);
                    } else {
                        throw new InvalidOperationException(
                            String.Format(
                                "Error while reversing the graph : Node {0} already added to {1}", 
                                toEdge, nodeConnections.Key));
                    }
                }
            }
        }

        private static bool ReadEdges(string line, out int from, out int to) {
            if (line == null || line.Trim() == string.Empty) {
                from = to = Int32.MinValue;
                return false;
            }

            string[] nodes = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (nodes == null || nodes.Length != 2) {
                throw new IOException(String.Format(
                    "Line : {0} : not in valid format in the input file.", line));
            }

            int fromEdge, toEdge;
            if (!Int32.TryParse(nodes[0], out fromEdge)) {
                from = to = Int32.MinValue;
                throw new IOException(String.Format(
                    "From Edge : {0} : cannot be parsed as int", nodes[0]));
            }

            if (!Int32.TryParse(nodes[1], out toEdge)) {
                from = to = Int32.MinValue;
                throw new IOException(String.Format(
                    "To Edge : {0} : cannot be parsed as int", nodes[1]));
            }

            from = fromEdge; to = toEdge;

            return true;
        }
    }
}
