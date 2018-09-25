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
        private List<Road> AllRoads = new List<Road>();
        private List<Road> RoadStack = new List<Road>();
        private List<Node> Path = new List<Node>();

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
            GetPaths(Grid.GetNodes[0], Grid.GetNodes[32]);
            Move(Start.x, 0.05, Start.z);
        }

        public void moveRobot()
        {
            GetShortestPath();
        }

        private int index = 0;
        private Road path;
        public void GetShortestPath()
        {
            //foreach (Road road in RoadStack)
            //{
            //    if (road.Node == Destination)
            //    {
            //        DestinationFound = true;
            //        break;
            //    }
            //}

            if (Visited.Last() != Destination)
            {
                foreach (Node node in Grid.GetDestinationsBySource(Visited.Last()))
                {
                    if (!Visited.Contains(node))
                        RoadStackAdd(node);
                }

                SortList(RoadStack);

                if (RoadStack.Count() > 0)
                {
                    Visited.Add(RoadStack.First().Node);
                    AllRoads.Add(RoadStack.First());
                    RoadStack.Remove(RoadStack.First());
                }

                SortList(RoadStack);
                GetShortestPath();
            }
            else
            {

                if (Path.Count == 0)
                {
                    index = RoadStack.Count - 1;
                    path = GetRoadByNode(Destination);
                }

                while (!done)
                {
                    if (path != null)
                    {
                        Path.Insert(0, path.Node);
                        path = path.PreviousNode;
                        index--;
                    }
                    else
                    {
                        done = true;
                        index = 0;
                    }
                }

                //index = 0;
                double nodeX = Path[index].x;
                double nodeZ = Path[index].z;

                double countX = x;
                countX = countInRange(countX, nodeX, Speed);

                double countZ = z;
                countZ = countInRange(countZ, nodeZ, Speed);

                Move(countX, 0.05, countZ);
                Console.WriteLine("Robot is at: X = {0}, Y = {1}, Z = {2}", x, y, z);

                if ((x == nodeX && z == nodeZ) && Destination != Path[index])
                {
                    index++;
                }
            }
        }



        private List<Road> RoadStackAdd(Node newNode)
        {
            bool update = true;
            if (RoadStack.Count != 0)
            {
                double distance = Grid.CalculateDistance(newNode, RoadStack.Last().Node);
                // Overwrite the node if it already exists
                for (int i = 0; i < RoadStack.Count; i++)
                {

                    if (RoadStack[i].Node.x == newNode.x && RoadStack[i].Node.z == newNode.z && GetRoadByNode(RoadStack[i].Node) != null)
                    {
                        if (Grid.CalculateDistance(RoadStack[i].Node, newNode) > distance)
                        {
                            RoadStack[i].Node = newNode;
                            return RoadStack;
                        }
                    }
                    else if (RoadStack[i].Node.x == newNode.x && RoadStack[i].Node.z == newNode.z && GetRoadByNode(RoadStack[i].Node) != null)
                    {
                        if (Grid.CalculateDistance(RoadStack[i].Node, newNode) <= distance)
                        {
                            update = false;
                        }
                    }
                }
                if (!update)
                {
                    return RoadStack;
                }

                // Add a new node
                Road road = new Road(newNode);
                road.AddPreviousRoad(GetRoadByNode(Visited.Last()));

                road.Distance = Grid.CalculateDistance(newNode, Visited.Last());
                //RoadStack.Add(road);
                RoadStack.Add(road);
            }
            else
            {
                Road road = new Road(newNode);
                road.Distance = Grid.CalculateDistance(newNode, Visited.Last());
                //RoadStack.Add(road);
                RoadStack.Add(road);
            }
            return RoadStack;
        }

        private void SortList(List<Road> list)
        {
            bool change = false;
            do
            {
                change = false;
                for (int i = 0; i < list.Count - 1; i++)
                {
                    if (list[i].Distance > list[i + 1].Distance)
                    {
                        Road temp;
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
            Move(start.x, 0.05, start.z);
            //Road begin = new Road(start);
            //begin.Distance = 0;
            //begin.AddPreviousRoad(null);
            //RoadStackAdd(start);
        }

        private Road GetRoadByNode(Node node)
        {
            foreach (Road road in AllRoads)
            {
                if (road.Node == node)
                {
                    return road;
                }
            }
            return null;
        }
    }

    class Road
    {
        public Node Node;
        public double Distance;
        public Road PreviousNode;

        public Road(Node node)
        {
            Node = node;
        }

        public void AddDistance(double distance)
        {
            Distance = distance;
        }

        public void AddPreviousRoad(Road road)
        {
            PreviousNode = road;
        }
    }
}
