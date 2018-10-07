using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Spaceships : Model3D, IUpdatable
    {
        private double ss_z = 125;
        private int spaceshipHeight = 25;
        private double stop = 8; 
        private double count = 0;
        private double radius = 0;
        private List<Product> Cargo = new List<Product>();

        //Summary of the constructor and more is found in Model3D.cs.
        public List<Product> cargo { get { return Cargo; } }

        public Spaceships(World world, string type, double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(world, type, x, y, z, rotationX, rotationY, rotationZ)
        {
            Console.WriteLine("Spaceship created");
        }

        /// <summary>
        /// This methode lets the spaceship go up, down and lets the spaceship rotate. 
        /// </summary>
        public void moveSpaceship()
        {
            if (ss_z != -140)
            {
                Move(x, spaceshipHeight + Math.Cos(radius) * 1.1, ss_z);
                Rotate(rotationX, radius / 2, rotationZ);
                radius += 0.25;
                radius = (radius >= 360) ? 0 : radius; // reset radius
                

                if (ss_z == stop && count < 100 || ss_z == -stop && count < 100)
                    count++;

                else
                {
                    ss_z -= 0.5;
                    count = 0;
                }
            }

            else // reset the spaceship to the initial position
            {
                reset();
            }
        }

        /// <summary>
        /// Checks whether the spaceship is on certain coordinates.
        /// </summary>
        /// <returns>true or false</returns>
        public bool checkCoordinates()
        {
            if (ss_z == stop || ss_z == -stop)
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Resets the position of the spaceship to z-coordinate 125.
        /// </summary>
        public void reset()
        {
            ss_z = 125;
            Move(x, spaceshipHeight, ss_z);
        }

        /// <summary>
        /// Adds the passed item to the list
        /// </summary>
        /// <param name="product">Product</param>
        public void AddCargo(Product product)
        {
            Cargo.Add(product);
        }
    }
}
