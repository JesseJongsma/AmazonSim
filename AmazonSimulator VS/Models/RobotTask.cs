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
        private RackMove rackmove = new RackMove(); 

        private bool loaded = false;
        private bool firstDestinationVisited;

        /// <summary>
        /// Sets the object task, robot and grid. 
        /// </summary>
        /// <param name="task">Task object</param>
        /// <param name="robot">Robots object</param>
        /// <param name="grid">Grid object</param>
        public RobotMove(Task task, Robots robot, Grid grid)
        {
            this.task = task;
            this.robot = robot;
            this.grid = grid;
        }

        /// <summary>
        /// This methode start the task by the given robot. 
        /// </summary>
        /// <param name="robot"></param>
        public void StartTask(Robots robot)
        {
            if (this.robot == robot)
            {
                //Checks where the robot is.
                robotLocation = grid.GetNodeByCoordinates(robot.x, robot.z);

                //If loaded is false, set firstDestination as destination and call the methode robot.InitPaths and sets the values robotlocation and the destination.
                if (!loaded)
                {
                    loaded = true;
                    destination = task.firstDestination;
                    robot.InitPaths(robotLocation, destination);
                }

                //If loaded is true than set final destination as destination and call the methode robot.initpaths and sets the values robotlocation and the destination.
                else
                {
                    destination = task.finalDestination;
                    robot.InitPaths(robotLocation, destination);
                }
                RunTask(robot);
            }
        }

        /// <summary>
        /// Runs de task and lets the robots move.
        /// </summary>
        /// <param name="robot">Robots object</param>
        public void RunTask(Robots robot)
        {
            //Checks if the given robot is this robot. 
            if (this.robot == robot)
            {
                //Calls the methode FollowPath so the robots moves.
                robot.FollowPath();
            }
        }

        /// <summary>
        /// Checks if the robot is at the first destination if so, set first destination visited to true. It will call the methode 
        /// task.getRack.moving so the rack will move along with the robot to the finaldestination. 
        /// </summary>
        /// <param name="robot">Robots object</param>
        /// <returns>True or false</returns>
        public bool TaskComplete(Robots robot)
        {
            if (this.robot == robot)
            {
                //Checks if robot x and z is at first destination x and z.
                if (robot.x == task.firstDestination.x && robot.z == task.firstDestination.z)
                {
                    firstDestinationVisited = true;
                    task.getRack.currentNode.occupied = false;
                    task.firstDestination.occupied = false;
                    task.finalDestination.occupied = true;
                    task.getRack.moving = true;
                }

                //If first destination visited is true than call the methode movingRack so the rack will also move along with the robot.
                if (firstDestinationVisited)
                {
                    task.getRack.moving = true;
                    rackmove.MovingRack(task.getRack, robot);
                }

                //If robot x and z is at finalDestination x and z than return true and set everything to false; 
                if ((robot.x == task.finalDestination.x && robot.z == task.finalDestination.z) && firstDestinationVisited)
                {
                    task.getRack.currentNode = grid.GetNodeByCoordinates(task.finalDestination.x, task.finalDestination.z);
                    task.getRack.moving = false;
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
        /// <summary>
        /// Lets the rack move along with the robot.
        /// </summary>
        /// <param name="rack">Racks object</param>
        /// <param name="robot">Robots object</param>
        public void MovingRack(Racks rack, Robots robot)
        {
            rack.x = robot.x;
            rack.z = robot.z;
        }
    }
}