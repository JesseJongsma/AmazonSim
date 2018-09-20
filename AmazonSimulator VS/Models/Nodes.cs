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
        private ConnectedNodes ConnectedNodes = new ConnectedNodes();
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

        public void AddConnection(Nodes source, Nodes destination)
        {
            double differenceX = source.GetX - destination.GetX;
            double differenceZ = source.GetZ - destination.GetZ;
            double distance = Math.Sqrt(Math.Pow(differenceX, 2) + Math.Pow(differenceZ, 2));
            ConnectedNodes.AddConnectedNode(source, destination, distance);
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
        public Nodes Destination { get; set; }
        public double Distance { get; set; }

        public void AddConnectedNode(Nodes source, Nodes destination, double distance)
        {
            Source = source;
            Destination = destination;
            Distance = distance;
        }
    }
}
