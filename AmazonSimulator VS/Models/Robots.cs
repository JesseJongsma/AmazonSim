using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Models
{
    public class Robots : Model3D, IUpdatable
    {
        private List<Node> Visited = new List<Node>();
        private List<Node> UnVisited = new List<Node>();
        private List<Node> NodesStack = new List<Node>();
        private Node Start;
        private Node Destination;
        private Grid Grid;
        private const double Speed = 0.2; // min = 0.1 max = 1 // Only use one decimal. // 0.5 is the recommended speed.
        bool done = false;

        public Robots(Grid grid, string type, double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(type, x, y, z, rotationX, rotationY, rotationZ)
        {
            Console.WriteLine("Robot created");
            Grid = grid;
            UnVisited = Grid.GetNodes;
            GetPaths(Grid.GetNodes[32], Grid.GetNodes[0]);
            GetPaths(Grid.GetNodes[32], Grid.GetNodes[0]);
            Move(Start.x, 0.05, Start.z);
        }

        public void moveRobot()
        {
            GetShortestPath();
        }

        private int index = 0;
        List<Node> Road = new List<Node>();
        public void GetShortestPath()
        {
            if (Visited.Last() == Destination || Road.Count != 0)
            {
                if (index < Road.Count || Road.Count == 0 && index == 0)
                {
                    while (!done)
                    {
                        if (Destination != Start)
                        {
                            Road.Insert(0, Destination);
                            Destination = Destination.previousNode;
                        }
                        else
                        {
                            done = true;
                        }
                    }

                    SortList(Road);
                    double nodeX = Road[index].x;
                    double nodeZ = Road[index].z;

                    double countX = x;
                    countX = countInRange(countX, nodeX, Speed);

                    double countZ = z;
                    countZ = countInRange(countZ, nodeZ, Speed);

                    Move(countX, 0.05, countZ);
                    Console.WriteLine("Robot is at: X = {0}, Y = {1}, Z = {2}", x, y, z);

                    if (x == nodeX && z == nodeZ)
                    {
                        
                        index++;
                    }
                }
            }
            else
            {
                foreach (Node node in Grid.GetDestinationsBySource(Visited.Last()))
                {
                    if (!Visited.Contains(node))
                        NodeStackAdd(node);
                }

                if (NodesStack.Count() > 0)
                {
                    Visited.Add(NodesStack.First());
                    NodesStack.Remove(NodesStack.First());
                }

                SortList(NodesStack);
                //SortList(Visited);

                GetShortestPath();
            }
        }



        private List<Node> NodeStackAdd(Node newNode)
        {
            bool update = true;
            if (NodesStack.Count != 0)
            {
                // Overwrite the node if it already exists
                for (int i = 0; i < NodesStack.Count; i++)
                {
                    if (NodesStack[i].x == newNode.x && NodesStack[i].z == newNode.z && NodesStack[i].distance > newNode.distance)
                    {
                        NodesStack[i] = newNode;
                        return NodesStack;
                    }
                    else if (NodesStack[i].x == newNode.x && NodesStack[i].z == newNode.z && NodesStack[i].distance < newNode.distance)
                    {
                        update = false;
                    }
                }
                if (!update)
                {
                    return NodesStack;
                }

                // Add a new node
                newNode.previousNode = Visited.Last();
                newNode.distance = Grid.CalculateDistance(newNode, newNode.previousNode) + newNode.previousNode.distance;
                NodesStack.Add(newNode);
            }
            else
            {
                newNode.previousNode = Visited.Last();
                newNode.distance = Grid.CalculateDistance(newNode, Visited.Last());
                NodesStack.Add(newNode);
            }
            return NodesStack;
        }

        private void SortList(List<Node> list)
        {
            bool change = false;
            do
            {
                change = false;
                for (int i = 0; i < list.Count - 1; i++)
                {
                    if (list[i].distance > list[i + 1].distance)
                    {
                        Node temp;
                        temp = list[i];
                        list[i] = list[i + 1];
                        list[i + 1] = temp;
                        change = true;
                    }
                }
            } while (change);
        }

        private double countInRange(double start, double end, double step)
        {
            start = Math.Round(start, 1);
            end = Math.Round(end, 1);
            step = Math.Round(step, 1);

            if (end < start)
                step = -step;

            if (start != end)
            {
                start += step;
            }
            return Math.Round(start, 1);
        }

        private void GetPaths(Node start, Node destination)
        {
            Start = start;
            Destination = destination;
            Visited.Add(start);
        }
    }
}
