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

        public RobotMove(Task task, Robots robot, Grid grid)
        {
            this.task = task;
            this.robot = robot;
            this.grid = grid;
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
                    destination = task.finalDestination;
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
        public Node finalDestination;
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