using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Racks : Model3D, IUpdatable
    {
        public Racks(string type, double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(type, x, y, z, rotationX, rotationY, rotationZ)
        {
            Console.WriteLine("Rack created");
        }

        public void moveRack()
        {
            Move(x, y, z);
        }
    }
}