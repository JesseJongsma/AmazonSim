using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{

    public class Doors : Model3D, IUpdatable
    {
        private List<Model3D> worldObjects = new List<Model3D>();
        private bool doorOpen = false;
        private double count; 

        //Summary of the constructor and more is found in Model3D.cs
        public Doors(World world, string type, double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(world, type, x, y, z, rotationX, rotationY, rotationZ)
        {
            Console.WriteLine("Doors created");
            worldObjects = world.worldObjects;
        }

        /// <summary>
        /// Checks by calling the methode check if a robot is nearby the door and if the door is open. And if true or false there will be
        /// a animation of opening the door and closing the door. 
        /// </summary>
        /// <param name="tick">Over how many milliseconds it has to check itself</param>
        /// <returns>true or false </returns>
        public override bool Update(int tick)
        {
            if (check() && !doorOpen)
            {
                count += 0.25;
                Move(x - 0.25, y, z);
                if(count == 4)
                {
                    doorOpen = true;
                    count = 0; 
                }
            }

            else if (!check() && doorOpen)
            {
                count += 0.25;
                Move(x + 0.25, y, z);
                if (count == 4)
                {
                    doorOpen = false;
                    count = 0;
                }
            }
            return true; 
        }

        /// <summary>
        /// Checks if a robot is anywhere to be found nearby the door.
        /// </summary>
        /// <returns>True or false</returns>
        public bool check()
        {
            foreach (Model3D model3d in worldObjects)
            {
                if (model3d is Robots)
                {
                    Robots robot = (Robots)model3d;
                    if (robot.x == -35 && (robot.z <= (z + 2.5) && robot.z >= (z - 2.5)))
                        return true;
                }
            }
            return false;
        }
    }
}