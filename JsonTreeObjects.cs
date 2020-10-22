using System.Collections.Generic;

/// Defines the the json structure as a C# object.
namespace JsonTree {
    public class Position {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class Node {

        public string Id { get; set; }
        public Position Position { get; set; }
        // Extra fields (not in original json) for pathfinding:
        public int f;
        public int g;
        public int h;
        public Node parent;

        public int calculateF() {
            f = g + h;
            return f;
        }
    }

    public class Edge {
        public string Id { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
    }

    public class Root {
        public Dictionary<string, Node> Nodes { get; set; }
        public Dictionary<string, Edge> Edges { get; set; }
    }
}