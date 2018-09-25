using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public interface IRobotTask
    {
        void StartTask(Robots r);
        bool TaskComplete(Robots r);
    }
    public class RobotMove : IRobotTask
    {
        private bool startupComplete = false;
        private bool complete = false;

        private Nodes[] path; 
        public RobotMove(Nodes[] path)
        {
            this.path = path;
        }

        public void StartTask(Robots r)
        {

        }

        public bool TaskComplete(Robots r)
        {
            return true; 
        }
    }

    public class RobotGetRack : IRobotTask
    {
        private bool startupComplete = false;
        private bool complete = false;

        private Nodes[] path;
        public RobotGetRack(Nodes[] path)
        {
            this.path = path;
        }

        public void StartTask(Robots r)
        {
            
        }

        public bool TaskComplete(Robots r)
        {
            return true; 
        }
    }
}