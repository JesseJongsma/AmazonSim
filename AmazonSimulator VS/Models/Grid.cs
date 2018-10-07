using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Grid
    {
        private List<Node> NodesList = new List<Node>();
        private List<ConnectedNodes> ConnectedNodesList = new List<ConnectedNodes>();

        /// <summary>
        /// Sets the x, z and the type of the node and adds this to the list NodesList.
        /// </summary>
        /// <param name="x">Coordinate x</param>
        /// <param name="z">COordinate z</param>
        /// <param name="type">The type of the Node</param>
        /// <returns>Node</returns>
        public Node AddNode(double x, double z, string type)
        {
            Node Node = new Node();

            Node.x = x;
            Node.z = z;
            Node.type = type;

            NodesList.Add(Node);
            return Node;
        }

        /// <summary>
        /// Add a new connection between 2 nodes
        /// </summary>
        /// <param name="source">source node</param>
        /// <param name="destination">target node</param>
        public void AddConnection(Node source, Node destination)
        {
            ConnectedNodes newConnection = new ConnectedNodes();

            newConnection.Source = source;
            newConnection.Destination = destination;

            ConnectedNodesList.Add(newConnection);
        }

        /// <summary>
        /// Find all nodes that are connected to a node
        /// </summary>
        /// <param name="searchNode"></param>
        /// <returns>Returns a list of all connected nodes</returns>
        public List<Node> GetDestinationsBySource(Node searchNode)
        {
            List<Node> destinations = new List<Node>();
            foreach (ConnectedNodes node in ConnectedNodesList)
            {
                if (node.Source == searchNode)
                    destinations.Add(node.Destination);
            }
            return destinations;
        }

        /// <summary>
        /// Calculate the distance between 2 nodes
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <returns>Returns the distance</returns>
        public double CalculateDistance(Node origin, Node destination)
        {
            double differenceX = origin.x - destination.x;
            double differenceZ = origin.z - destination.z;
            double distance = Math.Sqrt(Math.Pow(differenceX, 2) + Math.Pow(differenceZ, 2));

            return distance;
        }

        private Node getNode;
        /// <summary>
        /// Checks where the x and z coordinate is on the grid. 
        /// </summary>
        /// <param name="x">Coordinate x of a object</param>
        /// <param name="z">Coordinate z of a object</param>
        /// <returns></returns>
        public Node GetNodeByCoordinates(double x, double z)
        {
            foreach (Node node in NodesList)
            {
                if (Math.Round(x, 2) == Math.Round(node.x, 2) && Math.Round(z, 2) == Math.Round(node.z, 2))
                    getNode = node; 
            }
            return getNode; 
        }

        /// <summary>
        /// Search a node that isn't occupied
        /// </summary>
        /// <param name="currentNode">The current node of a rack</param>
        /// <returns>Returns an non-occupied node</returns>
        public Node GetAvailableNode(Node currentNode, string typeNode = "")
        {
            if(typeNode == "")
                typeNode = (currentNode.type == "cargoNode") ? "storageNode" : "cargoNode";

            for (int i = 0; i < NodesList.Count - 1; i++)
            {
                if (NodesList[i].type == typeNode && !NodesList[i].occupied)
                    return NodesList[i];

            }
            return null;
        }

        public List<Node> GetNodes
        {
            get
            {
                return NodesList;
            }
        }

        public List<ConnectedNodes> GetConnectedNodes
        {
            get
            {
                return ConnectedNodesList;
            }
        }
    }

    /// <summary>
    /// Object Node which have the variables x, y, type and occupied.
    /// </summary>
    public class Node
    {
        public double x;
        public double z;
        public string type;
        public bool occupied = false;
    }

    /// <summary>
    /// This object has object Node Source and object Node Destination.
    /// </summary>
    public class ConnectedNodes
    {
        public Node Source { get; set; }
        public Node Destination { get; set; }
    }
}
