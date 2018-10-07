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

        //Summary of the constructor and more is found in Model3D.cs
        public Racks(World world, string type, double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(world, type, x, y, z, rotationX, rotationY, rotationZ)
        {
            Console.WriteLine("Rack created");
        }

        /// <summary>
        /// Calls move in Model3D and gives the coordinates of this x,y and z.
        /// </summary>
        public void moveRack()
        {
            Move(x, y, z);
        }

        /// <summary>
        /// Adds a the given object 'product' to the list Contains. This because a rack can contain more than one product.
        /// </summary>
        /// <param name="product"></param>
        public void AddProduct(Product product)
        {
            Contains.Add(product);
        }

        /// <summary>
        /// This removes the amount of products that is given along with it. 
        /// </summary>
        /// <param name="product">Product object</param>
        /// <param name="amount">Amount of product</param>
        /// <returns>Object Task or null</returns>
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
        }

        /// <summary>
        /// Creates a task and sets first destination of the tasks and the final destination of the task. 
        /// </summary>
        /// <returns>Object task</returns>
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