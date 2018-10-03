using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public interface IRobotTask
    {
        void StartTask(Robots r);
        void RunTask(Robots r);
        bool TaskComplete(Robots r);
    }
    public class RobotMove : IRobotTask
    {
        private Node robotLocation;
        private Task task;
        private Grid grid;
        private Robots robot;
        private Node destination;
        RackMove rackmove = new RackMove(); 
        private bool loaded = false;
        private bool firstDestinationVisited;
        private char symbol; 

        public RobotMove(Task task, Robots robot, Grid grid, char teken)
        {
            this.task = task;
            this.robot = robot;
            this.grid = grid;
            this.symbol = teken; 
        }

        public void StartTask(Robots robot)
        {
            if (this.robot == robot)
            {
                robotLocation = grid.GetNodeByCoordinates(robot.x, robot.z);
                if (!loaded)
                {
                    loaded = true;
                    destination = task.firstDestination;
                    robot.InitPaths(robotLocation, destination);
                }
                else if (robotLocation == destination)
                {
                    destination = task.finialDestination;
                    robot.InitPaths(robotLocation, destination);
                }
                RunTask(robot);
            }
        }

        public void RunTask(Robots robot)
        {
            if (this.robot == robot)
            {
                robot.FollowPath();
            }
        }

        public bool TaskComplete(Robots robot)
        {
            if (this.robot == robot)
            {
                if (robot.x == task.firstDestination.x && robot.z == task.firstDestination.z)
                {
                    firstDestinationVisited = true;
                }
                if (firstDestinationVisited)
                {
                    rackmove.MovingRack(task.getRack, robot);
                }

                if ((robot.x == task.finialDestination.x && robot.z == task.finialDestination.z) && firstDestinationVisited)
                {
                    Console.WriteLine(symbol * 2);
                    robot.Move(robot.x, robot.y, robot.z + (symbol * 2));
                    rackmove.MovingRack(task.getRack, robot);
                    loaded = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }

    public class Task
    {
        public Node firstDestination;
        public Node finialDestination;
        public Racks getRack;
    }

    public class RackMove
    {
        public void MovingRack(Racks rack, Robots robot)
        {
            rack.x = robot.x;
            rack.z = robot.z; 
        }
    }
}