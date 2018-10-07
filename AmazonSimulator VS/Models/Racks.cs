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
        public bool moving = false;


        public Racks(World world, string type, double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(world, type, x, y, z, rotationX, rotationY, rotationZ)
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

        public Task RemoveStock(Product product, int amount)
        {
            for (int i = 0; i < Contains.Count(); i++)
            {
                if (Contains[i] == product)
                {
                    if (product.stock >= amount)
                    {
                        Contains[i].RemoveStock(amount);
                        if (product.stock == 0)
                        {
                            Contains.Remove(product);
                            if (Contains.Count() > 0)
                                return CreateTask();
                            else
                            {
                                this.attr = "deleted";
                            }
                        }
                        else
                        {
                            return CreateTask();
                        }
                        break;
                    }
                    
                }
            }
            return null;

            //if (Contains.Count() == 0)
            //{
                
            //}
        }

        private Task CreateTask()
        {
            Task task = new Task();
            task.firstDestination = currentNode;
            task.finalDestination = world.grid.GetAvailableNode(currentNode);
            task.getRack = this;
            return task;
        }

        public List<Product> contains { get { return Contains; } }

    }


}