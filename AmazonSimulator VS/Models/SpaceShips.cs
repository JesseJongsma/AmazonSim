using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Spaceships : Model3D, IUpdatable
    {
        public Spaceships(string type, double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base (type,x,y,z,rotationX,rotationY,rotationZ)
        {
            Console.WriteLine("Spaceship created");
        }
    }
}
