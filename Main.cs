using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using JsonTree;

namespace SlingshotSolution {

    class Program {

        private string myJson;
        private List<Node> openList;
        private List<Node> closedList;
        private Root deserializedJson;

        /// Main method which reads the json graph and executes pathfinding.
        static void Main(string[] args) {
            Program p = new Program();
            // Calls funcion to read in the json to a .NET object ("object" or "dynamic")
            p.ReadJson("object");
            // Validates that the graph has only NSEW movement:
            //p.validateJson();
            string solution = p.Pathfinding(0, 0, 60, 40);
            Console.WriteLine(solution);
        }

        /// Calculates the shortest path using A* algorithm.
        public string Pathfinding(int initialX, int initialY, int goalX, int goalY) {
            openList = new List<Node>();
            closedList = new List<Node>();
            int g = 0;

            // Defines initial and goal nodes:
            Node initialNode = deserializedJson.Nodes["Node" + initialX/10 + "_" + initialY/10];
            Node goalNode = deserializedJson.Nodes["Node" + goalX/10 + "_" + goalY/10];

            // Sets g to infinity and calculates f for all nodes:
            foreach(KeyValuePair<string, JsonTree.Node> node in deserializedJson.Nodes) {
                node.Value.g = int.MaxValue;
                node.Value.calculateF();
                node.Value.parent = null;
            }

            // Add starting node to the list:
            openList.Add(initialNode);

            // While there are still nodes to be searched, do algorithm. This will run until there is no longer a node closer to the goal,
            // ie. the goal has been reached, or no path was found.
            while (openList.Count > 0) {
                // Initialise list of neighbour nodes and current node:
                List<Node> neighbourList = new List<Node>();
                Node currentNode = FindMinFNode(openList);
                closedList.Add(currentNode);
                openList.Remove(currentNode);

                Console.WriteLine(currentNode.Id);

                // Find neighbors
                // West:
                if (currentNode.Position.X - 10 >= 0) {
                    neighbourList.Add(deserializedJson.Nodes["Node" + (currentNode.Position.X - 10)/10 + "_" + (currentNode.Position.Y)/10]);
                }
                // East:
                if (currentNode.Position.X + 10 < 100) {
                    neighbourList.Add(deserializedJson.Nodes["Node" + (currentNode.Position.X + 10)/10 + "_" + (currentNode.Position.Y)/10]);
                }
                // South:
                if (currentNode.Position.Y - 10 >= 0) {
                    neighbourList.Add(deserializedJson.Nodes["Node" + (currentNode.Position.X)/10 + "_" + (currentNode.Position.Y - 10)/10]);
                }
                // North:
                if (currentNode.Position.Y + 10 < 100) {
                    neighbourList.Add(deserializedJson.Nodes["Node" + (currentNode.Position.X)/10 + "_" + (currentNode.Position.Y + 10)/10]);
                }

                // Increment g (distance from start):
                g += 10;

                foreach (Node neighbour in neighbourList) {
                    // if node has already been searched, skip:
                    if (closedList.Contains(neighbour)) {
                        continue;
                    }
                    // else find all neighbors and calculate their f value:
                    if (g < neighbour.g) {
                        neighbour.g = g;
                        neighbour.h = CalculateManhattanDistance(neighbour, goalNode);
                        neighbour.calculateF();
                        neighbour.parent = currentNode;
                        openList.Add(neighbour);
                    }
                }
            }
            // Now we are out of nodes on the open list.

            // Trace back through the nodes by evaluating the parent of each:
            Node current = goalNode;

            string dataStream = current.Id + ", ";

            // While node still has a parent (while list still contains nodes), cycle back through nodes and add them to a string for the result:
            while (current.parent != null) {
                current = current.parent;
                Console.WriteLine("Current Node - x: " + current.Position.X + ", y: " + current.Position.Y);
                dataStream += current.Id + ", ";
            }

            return dataStream;
        }

        /// Calculates the "city-block" distance between two nodes.
        public int CalculateManhattanDistance(Node A, Node B) {
            // Manhattan Distance = Difference in x coordinates + difference in y coordinates:
            return (int)(Math.Abs(A.Position.X - B.Position.X) + (A.Position.Y - B.Position.Y));
        }

        /// Finds the Node with the lowest f value from a list and returns it.
        public Node FindMinFNode(List<Node> list) {
            Node fMinNode = list[0];
            foreach(Node node in openList) {
                if (node.calculateF() < fMinNode.calculateF()) {
                    fMinNode = node;
                }
            }
            return fMinNode;
        }

        /// Reads input JSON and deserializes it to a C# object or dynamically based on argument
        /// "object" = C# object
        /// "dynamic" = at runtime
        public Root ReadJson(string type) {
            try {
                StreamReader r = new StreamReader("BasicJSON.json");
                myJson = r.ReadToEnd();
                if (type == "object") {
                    // Deserialize json
                    deserializedJson = JsonSerializer.Deserialize<Root>(myJson);
                    // Re-serialize and print to test that deserialisation worked
                    var testJson = JsonSerializer.Serialize(deserializedJson);
                    // Test value
                    int x = 8;
                    //Console.WriteLine(testJson);
                    //Console.WriteLine(deserializedJson.Nodes["Node0_" + x].Position.Y);
                    return deserializedJson;
                } else if (type == "dynamic") {
                    deserializedJson = JsonSerializer.Deserialize<dynamic>(myJson);
                    return deserializedJson;
                }
            } catch(Exception e) {
                Debug.Write("Error reading input JSON: " + e.Message.ToString());
                Console.WriteLine("ERROR");
            }
            return null;
        }

        /// Confirms that the graph only has 4-directional (NSWE) motion. Validates that the A* heuristic is admissable.
        public void ValidateJson() {
            var inputJson = ReadJson("object");            
            // Validate that a node only has (x + 1, x - 1, y + 1, y - 1) edges, ie purely NSEW movement.
            foreach(KeyValuePair<string, JsonTree.Edge> edge in inputJson.Edges) {
                // Store current Edge source and target combinations as integers.
                int src_x = edge.Value.Source[4] - '0', tgt_x = edge.Value.Target[4] - '0', src_y = edge.Value.Source[6] - '0', tgt_y = edge.Value.Target[6] - '0';
                if (tgt_x == (src_x + (1)) || tgt_x == (src_x - 1)) {
                    //Console.WriteLine("East/West Edge");
                } else if (tgt_y == (src_y + (1)) || tgt_y == (src_y - 1)) {
                    //Console.WriteLine("North/South Edge");
                } else {
                    Console.WriteLine("Movement is not purely NSWE");
                }
            } 
        }
    }
}
