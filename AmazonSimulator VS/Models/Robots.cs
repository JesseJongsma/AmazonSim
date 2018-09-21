using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Robots : Model3D, IUpdatable
    {
        private int c = 0;
        private List<Nodes> Visited = new List<Nodes>();
        private List<Nodes> UnVisited = new List<Nodes>();
        private double Speed = 1;
        public Nodes Nodes;

        public Robots(Nodes nodes, string type, double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(type, x, y, z, rotationX, rotationY, rotationZ)
        {
            Console.WriteLine("Robot created");
            Nodes = nodes;
            UnVisited = Nodes.GetNodes;
            //Move(UnVisited.First().GetX, 0.05, UnVisited.First().GetZ);
            //transferNode(UnVisited.First());
        }

        public void moveRobot()
        {
            Move(x, 0.05, z);
        }

        public void GetShortestPath()
        {

        }

        private void GetPaths()
        {

        }

        private void transferNode(Nodes node)
        {
            if (Visited.Contains(node))
            {
                UnVisited.Add(node);
                Visited.Remove(node);
            }
            else if (UnVisited.Contains(node))
            {
                Visited.Add(node);
                UnVisited.Remove(node);
            }
        }
    }
}
