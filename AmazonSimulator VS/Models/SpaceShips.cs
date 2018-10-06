﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Spaceships : Model3D, IUpdatable
    {
        private double ss_z = 125;
        private double stop = 8; 
        private double count = 0;
        private double radius = 0;

        //Summary of the constructor and more is found in Model3D.cs
        public Spaceships(string type, double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(type, x, y, z, rotationX, rotationY, rotationZ)
        {
            Console.WriteLine("Spaceship created");
        }

        public void moveSpaceship()
        {
            int height = 25;
            if (ss_z != -140)
            {

                Move(x, height + Math.Cos(radius) * 1.1, ss_z);
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
                ss_z = 125;
                Move(x, height, ss_z);
            }
        }


        public bool checkCoordinates()
        {
            if (ss_z == stop || ss_z == -stop)
            {
                return true;
            }
            else
                return false;
        }
    }
}
