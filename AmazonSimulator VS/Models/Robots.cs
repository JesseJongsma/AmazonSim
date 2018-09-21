using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Robots : Model3D, IUpdatable
    {
        private int c = 0;
        public Robots(string type, double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(type, x, y, z, rotationX, rotationY, rotationZ)
        {
            Console.WriteLine("Robot created");
        }

        public void moveRobot(Model3D model)
        {
            model.Move(model.x, model.y, model.z);
            //if (model.type == "robot")
            //{
            //    c++;
            //    if (c > 100)
            //        c = 0;

            //    model.Move(c, 0, c);
            //}
        }

        public void DijkstraAlgorithm()
        {

        }
    }
}
