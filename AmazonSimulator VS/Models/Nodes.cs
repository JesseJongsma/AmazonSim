using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Nodes
    {
        private List<Nodes> NodesList = new List<Nodes>();
        private List<ConnectedNodes> ConnectedNodesList = new List<ConnectedNodes>();
        private ConnectedNodes _ConnectedNodes = new ConnectedNodes();
        private double _x;
        private double _z;

        public Nodes AddNode(double x, double z)
        {
            Nodes Node = new Nodes();
            Node._x = x;
            Node._z = z;

            NodesList.Add(Node);
            return Node;
        }

        //public void AddSource(Nodes source)
        //{
        //    ConnectedNodes connectedNodes = new ConnectedNodes();
        //    connectedNodes.Source = source;
        //    ConnectedNodesList.Add(connectedNodes);
        //}

        public void AddConnection(Nodes source, Nodes destination)
        {
            // Calculate distance
            double differenceX = source.GetX - destination.GetX;
            double differenceZ = source.GetZ - destination.GetZ;
            double distance = Math.Sqrt(Math.Pow(differenceX, 2) + Math.Pow(differenceZ, 2));

            ConnectedNodes newConnection = new ConnectedNodes();
            newConnection.AddConnection(source, destination, distance);
            ConnectedNodesList.Add(newConnection);
        }

        public Nodes GetConnection(Nodes searchNode)
        {
            foreach (ConnectedNodes node in ConnectedNodesList)
            {
                if (node.GetDestinations(searchNode) != null)
                    return node.GetDestinations(searchNode);
            }
            return null;
        }

        public int length()
        {
            int count = 0;
            foreach (Nodes node in NodesList)
                count++;

            return count;
        }

        public List<Nodes> GetNodes
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

        public double GetX
        {
            get
            {
                return _x;
            }
        }

        public double GetZ
        {
            get
            {
                return _z;
            }
        }
    }

    public class ConnectedNodes 
    {
        public Nodes Source { get; set; }
        public List<Nodes> Destinations = new List<Nodes>();
        public double Distance { get; set; }

        public void AddConnection(Nodes source, Nodes destination, double distance)
        {
            Source = source;
            Destinations.Add(destination);
            Distance = distance;
        }

        public Nodes GetDestinations(Nodes searchNode)
        {
            foreach (Nodes node in Destinations)
            {
                if (node == searchNode)
                    return node;
            }
            return null;
        }
    }
}
