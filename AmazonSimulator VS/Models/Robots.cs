using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Robots : Model3D, IUpdatable
    {
        private int c = 0;
        //private List<IRobotTask> tasks = new List<IRobotTask>(); 
        private List<Nodes> Visited = new List<Nodes>();
        private List<Nodes> UnVisited = new List<Nodes>();
        private Nodes Start;
        private Nodes Destination;
        private double Speed = 0.1;
        public Nodes _Nodes;

        public Robots(Nodes nodes, string type, double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(type, x, y, z, rotationX, rotationY, rotationZ)
        {
            Console.WriteLine("Robot created");
            _Nodes = nodes;
            UnVisited = _Nodes.GetNodes;
            //Move(UnVisited.First().GetX, 0.05, UnVisited.First().GetZ);
            //transferNode(UnVisited.First());
        }

        private double count = 0;
        private bool first = false;
        public void moveRobot()
        {
            GetShortestPath();
            if (!first)
            {
                Move(Start.GetX, 0.05, Start.GetZ);
                first = true;
            }

            if (count > 1)
            {
                Move(x, 0.05, z);
                count = 0;
            }
            else
            {
                count = count + Speed;
            }
        }

        public void GetShortestPath()
        {
            GetPaths(new Nodes(), new Nodes());
            UnVisited = _Nodes.GetNodes;
            Visited.Add(Start);

            foreach (ConnectedNodes unVisited in _Nodes.GetConnectedNodes)
            {
                foreach (Nodes destination in unVisited.Destinations)
                {
                    Console.WriteLine(_Nodes.CalculateDistance(Visited.Last(), destination));
                }
            }
        }

        private void GetPaths(Nodes start, Nodes destination)
        {
            Start = _Nodes.GetNodes[1];
            Destination = _Nodes.GetNodes[10];
        }

        //public override bool Update(int tick)
        //{
        //    if (tasks != null)
        //    {
        //        if (tasks.First().TaskComplete(this))
        //        {
        //            tasks.RemoveAt(0);
        //            if (tasks.Count == 0)
        //            {
        //                tasks = null;
        //            }
        //            tasks.First().StartTask(this);
        //        }
        //        return true;
        //    }
        //    else
        //        return false; 
        //}
    }
}
