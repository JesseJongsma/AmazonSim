using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class HumanRobot : Model3D, IUpdatable
    {
        public HumanRobot(string type, double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(type, x, y, z, rotationX, rotationY, rotationZ)
        {
            Console.WriteLine("HumanRobot created, hallo, ik ben de robot, ik ben misschien een beetje groot maar ik zorg ervoor dat de pakketjes van de robot naar het ophaalpunt voor de ufo gaan :}");
        }
    }
}
