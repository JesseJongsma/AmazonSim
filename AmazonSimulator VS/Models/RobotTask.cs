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
        private Node node; 
        private Task task;
        private Grid grid;
        private Robots robot;
        private bool loaded = false; 

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
                node = grid.GetNodeByCoordinates(robot.x, robot.z);
                if (loaded == false)
                {
                    loaded = true; 
                    robot.InitPaths(node, task.firstDestination);
                }
                else if (node == task.firstDestination)
                {
                    robot.InitPaths(task.firstDestination, task.finialDestination);
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
                if (robot.x == task.finialDestination.x && robot.z == task.finialDestination.z)
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
        public Node finialDestination;
    }

}