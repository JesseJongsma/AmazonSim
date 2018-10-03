using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Racks : Model3D, IUpdatable
    {
        private List<Product> Contains = new List<Product>();
        public Node currentNode;


        public Racks(string type, double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(type, x, y, z, rotationX, rotationY, rotationZ)
        {
            Console.WriteLine("Rack created");
        }

        public void moveRack()
        {
            Move(x, y, z);
        }

        public void AddProduct(Product product)
        {
            Contains.Add(product);
        }

        //public

        public List<Product> contains { get { return Contains; } }

    }


}