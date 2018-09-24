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

        public Node AddNode(double x, double z)
        {
            Node Node = new Node();

            Node.x = x;
            Node.z = z;

            NodesList.Add(Node);
            return Node;
        }

        public void AddConnection(Node source, Node destination)
        {
            ConnectedNodes newConnection = new ConnectedNodes();

            newConnection.Source = source;
            newConnection.Destination = destination;

            ConnectedNodesList.Add(newConnection);
        }

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

        public double CalculateDistance(Node origin, Node destination)
        {
            double differenceX = origin.x - destination.x;
            double differenceZ = origin.z - destination.z;
            double distance = Math.Sqrt(Math.Pow(differenceX, 2) + Math.Pow(differenceZ, 2));

            return distance;
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

    public class Node
    {
        public double x;
        public double z;
        public Node previousNode = null;
        public double distance = 0;
    }

    public class ConnectedNodes 
    {
        public Node Source { get; set; }
        public Node Destination { get; set; }
    }
}
