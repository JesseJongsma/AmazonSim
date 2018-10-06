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
        private List<Model3D> worldObjects = new List<Model3D>();
        public RobotMove robotMove = null;
        
        private Node Start;
        private Node Destination;
        private Grid Grid;
        private Inventory _Inventory;
        private const double Speed = 0.1; // min = 0.1 max = 1 // Only use one decimal. 
        //bool done = false;

        //Summary of the constructor and more is found in Model3D.cs
        public Robots(World world, string type, double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(world, type, x, y, z, rotationX, rotationY, rotationZ)
        {
            Console.WriteLine("Robot created");
            Grid = world.grid;
            _Inventory = world.Inventory;
            worldObjects = world.worldObjects; 
        }

        /// <summary>
        /// Sets the start node and the destination node and adds start to the list visited. Calls the methode AddRoad and gives start to it.
        /// </summary>
        /// <param name="start">Node object</param>
        /// <param name="destination">Node object</param>
        public void InitPaths(Node start, Node destination)
        {
            Start = start;
            Destination = destination;
            UnVisited = Grid.GetNodes;
            Visited.Add(start);
            AddRoad(Start);
        }

        private Robots destined;
        private double closestTo; 
        /// <summary>
        /// Checks which robot is the closest to the rack and gives the task to the closest robot.
        /// </summary>
        public void giveTask()
        {
            if (_Inventory.Tasks.Count != 0)
            {
                //Sets for every robot the distance to the rack
                foreach (Model3D model3d in worldObjects)
                {
                    if (model3d is Robots)
                    {
                        Robots robot = (Robots)model3d;
                        double closestToX = robot.x - _Inventory.Tasks.First().getRack.x;
                        double closestToZ = robot.z - _Inventory.Tasks.First().getRack.z;
                        robot.closestTo = Math.Sqrt(Math.Pow(closestToX, 2) + Math.Pow(closestToZ, 2));
                    }
                }

                //Checking which robot is the closest 
                foreach(Model3D model3d in worldObjects)
                {
                    if(model3d is Robots)
                    {
                        Robots robot = (Robots)model3d;
                        if (robot.closestTo < this.closestTo || robot.closestTo == this.closestTo)
                        {
                            this.closestTo = robot.closestTo; 
                            destined = robot;
                        }
                    }
                }

                //Is giving the robot the task
                if (robotMove == null && destined == this && !_Inventory.Tasks.First().getRack.moving)
                {
                    robotMove = new RobotMove(_Inventory.Tasks.First(), this, Grid);
                    _Inventory.Tasks.RemoveAt(0);
                }
                destined = null;
            }
        }

        /// <summary>
        /// Overrides the methode Update from the class Model3D. 
        /// </summary>
        /// <param name="tick">Over how many milliseconds it has to check itself</param>
        /// <returns>true or false</returns>
        public override bool Update(int tick)
        {
            giveTask();

            if (robotMove != null)
            {
                if (robotMove.TaskComplete(this))
                {
                    Console.WriteLine("override bool update");

                    robotMove = null;
                    return false;
                }
                else if (!robotMove.TaskComplete(this))
                {
                    robotMove.StartTask(this);
                }
                return true;
            }
            else
                return false;
        }
        
        /// <summary>
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        /// <returns></returns>
        private List<Node> GeneratePath()
        {
            bool done = false;
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
        /// <summary>
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        /// <returns></returns>
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
        private double nodeX, nodeZ, countX, countZ;
        private List<Node> path;
        /// <summary>
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        public void FollowPath()
        {
            if (path == null)
                path = GetShortestPath();

            nodeX = path[index].x;
            nodeZ = path[index].z;

            countX = x;
            countX = countInRange(countX, nodeX, Speed);

            countZ = z;
            countZ = countInRange(countZ, nodeZ, Speed);

            Move(countX, 0.05, countZ);
            if (x == nodeX && z == nodeZ)
            {
                if (index == path.Count -1)
                {
                    index = 0;
                    Reset();
                }
                else
                {
                    if (index != path.Count - 1)
                        index++;
                }
            }

        }

        /// <summary>
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        /// <param name="newNode"></param>
        private void RoadStackAdd(Node newNode)
        {
            if (RoadStack.Count != 0)
            {
                if (UpdateDistance(RoadStack, newNode))
                {
                    return;
                }
            }

            RoadStack.Add(AddRoad(newNode));
        }

        /// <summary>
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private Road AddRoad(Node node)
        {
            Road road = new Road(node);

            if (Visited.Last() == node)
            {
                road.PreviousNode = null;
                road.Distance = Grid.CalculateDistance(node, Visited.Last());
            }
            else
            {
                road.PreviousNode = GetRoadByNode(Visited.Last());
                road.Distance = Grid.CalculateDistance(node, Visited.Last()) + road.PreviousNode.Distance;
            }

            AllRoads.Add(road);
            return road;
        }

        /// <summary>
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        /// <param name="list"></param>
        /// <param name="newNode"></param>
        /// <returns></returns>
        private bool UpdateDistance(List<Road> list, Node newNode)
        {
            bool updated = false;
            double distance = Grid.CalculateDistance(newNode, Visited.Last());
            for (int i = 0; i < RoadStack.Count; i++)
            {
                if (RoadStack[i].Node == newNode)
                {
                    if (Grid.CalculateDistance(RoadStack[i].Node, Visited.Last()) > distance)
                    {
                        RoadStack[i].Node = newNode;
                        updated = true;
                    }
                }
            }

            return updated;
        }

        /// <summary>
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        /// <param name="list"></param>
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

        /// <summary>
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <returns></returns>
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

        /// <summary>
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Reset methode, resets everyting to null; 
        /// </summary>
        private void Reset()
        {
            Start = null;
            Destination = null;
            UnVisited = Grid.GetNodes;
            Visited = new List<Node>();
            RoadStack = new List<Road>();
            AllRoads = new List<Road>();
            path = null;
        }
    }

    /// <summary>
    /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
    /// </summary>
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
