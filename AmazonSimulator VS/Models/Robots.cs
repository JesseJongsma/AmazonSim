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
        //private List<Node> Path = new List<Node>();
        public List<RobotMove> tasks = new List<RobotMove>();

        private Node Start;
        private Node Destination;
        private Grid Grid;
        private const double Speed = 0.1; // min = 0.1 max = 1 // Only use one decimal. // 0.5 is the recommended speed.
        bool done = false;

        public Robots(World world, string type, double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(type, x, y, z, rotationX, rotationY, rotationZ)
        {
            Console.WriteLine("Robot created");
            Grid = world.grid;
            //InitPaths(Grid.GetNodes[0], Grid.GetNodes[32]);
        }

        public void InitPaths(Node start, Node destination)
        {
            Start = start;
            Destination = destination;
            UnVisited = Grid.GetNodes;
            Visited.Add(start);
            AddRoad(Start);
        }

        public override bool Update(int tick)
        {
            if (needsUpdate)
            {
                if (tasks != null)
                {
                    if (tasks.First().TaskComplete(this))
                    {
                        Console.WriteLine("override bool update");
                        tasks.RemoveAt(0);
                        if (tasks.Count == 0)
                        {
                            tasks = null;
                        }

                        Console.WriteLine("StartTask");
                    }
                    else if (!tasks.First().TaskComplete(this))
                    {
                        tasks.First().StartTask(this);
                    }
                }
                needsUpdate = true;
                return true;
            }
            return false;
        }

        public void moveRobot()
        {
            FollowPath();
        }

        private List<Node> GeneratePath()
        {
            //int index = ;
            List<Node> path = new List<Node>();
            Road DestinationRoad = GetRoadByNode(Destination);

            while (!done)
            {
                if (DestinationRoad != null)
                {
                    path.Insert(0, DestinationRoad.Node);
                    DestinationRoad = DestinationRoad.PreviousNode;
                }
                else
                {
                    done = true;
                }
            }

            return path;
        }

        //private Road path;
        private List<Node> GetShortestPath()
        {
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
                    //AllRoads.Add(RoadStack.First());
                    RoadStack.Remove(RoadStack.First());
                }

                SortList(RoadStack);
                return GetShortestPath();
            }
            else
            {
                return GeneratePath();
            }
        }

        private int index = 0;
        private List<Node> path;
        public void FollowPath()
        {
            if (path == null)
                path = GetShortestPath();

            double nodeX = path[index].x;
            double nodeZ = path[index].z;

            double countX = x;
            countX = countInRange(countX, nodeX, Speed);

            double countZ = z;
            countZ = countInRange(countZ, nodeZ, Speed);

            Move(countX, 0.05, countZ);
            Console.WriteLine("Robot is at: X = {0}, Y = {1}, Z = {2}", x, y, z);

            if ((x == nodeX && z == nodeZ) && Destination != path[index])
            {
                index++;
            }
            else if ((x == nodeX && z == nodeZ) && Destination == path[index])
            {
                Reset();
            }
        }

        private void RoadStackAdd(Node newNode)
        {
            if (RoadStack.Count != 0)
            {
                if (!UpdateDistance(RoadStack, newNode))
                {
                    return;
                }
            }


            RoadStack.Add(AddRoad(newNode));
        }

        private Road AddRoad(Node node)
        {
            Road road = new Road(node);
            road.Distance = Grid.CalculateDistance(node, Visited.Last());
            road.PreviousNode = (Visited.Last() == node) ? null : GetRoadByNode(Visited.Last());
            AllRoads.Add(road);
            return road;
        }

        private bool UpdateDistance(List<Road> list, Node newNode)
        {
            bool updated = true;
            double distance = Grid.CalculateDistance(newNode, RoadStack.Last().Node);
            for (int i = 0; i < RoadStack.Count; i++)
            {
                if (RoadStack[i].Node.x == newNode.x && RoadStack[i].Node.z == newNode.z && GetRoadByNode(RoadStack[i].Node) != null)
                {
                    if (Grid.CalculateDistance(RoadStack[i].Node, newNode) > distance)
                    {
                        RoadStack[i].Node = newNode;
                    }
                    else if (Grid.CalculateDistance(RoadStack[i].Node, newNode) <= distance)
                    {
                        updated = false;
                    }
                }
            }
            return updated;
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

        private void Reset()
        {
            Start = null;
            Destination = null;
            UnVisited = Grid.GetNodes;
            Visited = new List<Node>();
            RoadStack = new List<Road>();
            AllRoads = new List<Road>();

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
