using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Trucks : Model3D, IUpdatable
    {
        public Trucks(string type, double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base (type,x,y,z,rotationX,rotationY,rotationZ)
        {
            Console.WriteLine("Truck created");
        }
    }
}
