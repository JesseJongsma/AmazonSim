using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Models
{
    public class Inventory
    {
        private List<Product> Products = new List<Product>(); // List of products
        private List<Product> Orders = new List<Product>(); // List of orders for our warehouse
        private List<Product> Shipments = new List<Product>(); // List of outgoing orders
        private List<Racks> Racks = new List<Racks>(); // List of all exsiting racks
        private World World;

        public List<Task> Tasks = new List<Task>(); // List of all tasks for the robots

        public List<Product> orders { get { return Orders; } }
        public List<Product> shipments { get { return Shipments; } }
        public List<Racks> racks { get { return Racks; } }

        /// <summary>
        /// Sets the object world.
        /// </summary>
        /// <param name="world">World instance</param>
        public Inventory(World world)
        {
            World = world;
        }

        /// <summary>
        /// Prompts the user to order products
        /// </summary>
        public void PromptUser()
        {
            string productName = "";
            int amount = 0;

            Console.WriteLine("Please enter your order.");
            Console.WriteLine("{product name} {amount}");

            string input = Console.ReadLine(); // User input
            string[] param = input.Split(' ');

            // Check if the input is valid
            if (param.Count() >= 1)
                productName = param[0];

            if (param.Count() == 2)
                int.TryParse(param[1], out amount);

            if (param.Count() > 2 || productName == "")
            {
                Console.WriteLine("Something went wrong with your input");
                return;
            }

            Product result = SearchProduct(productName); // Search the entered product
            Racks rack = SearchRackByProduct(productName); // Search the rack

            if (rack != null)
            {
                if (amount <= result.stock)
                {
                    // Create a copy of the product to be shipped
                    Product productToShip = new Product();
                    productToShip = productToShip.Clone(result);

                    // Remove stock from the copy and add to shipments
                    productToShip.RemoveStock(productToShip.stock - amount);
                    AddOrderOrShipment(Shipments, productToShip);

                    // Start new tread to add the task
                    Console.WriteLine("Ordered {0} of {1}", amount, productName);
                    Thread inventoryThread = new Thread(() => AddTask(rack, "cargoNode"));
                    inventoryThread.Start();
                }
                else
                    Console.WriteLine("Your order is too large or too small.");
            }
            else if(result != null && param.Count() == 2)
            {
                Console.WriteLine("Rack couldn't be found, try again later.");
            }

            if (param.Count() == 1 && productName == "list")
            {
                ShowStock();
            }
            else if (result == null)
            {
                bool correct = false;
                while (!correct)
                {
                    Console.WriteLine("{0}, was not found. Do you want to add this item to the invetory? (Y/N)", productName);
                    string AddProduct = Console.ReadLine();
                    if (AddProduct.ToLower().ToString() == "y")
                    {
                        this.AddProduct(productName);
                        correct = true;
                    }
                    else if (AddProduct.ToLower().ToString() == "n")
                    {
                        correct = true;
                    }
                }
            }
        }

        /// <summary>
        /// Displays for every product how many there are of the product.
        /// </summary>
        private void ShowStock()
        {
            foreach (Product product in Products)
            {
                Console.WriteLine("{0} of {1}", product.stock, product.name);
            }
        }

        /// <summary>
        /// Adds a new task for the robots
        /// </summary>
        /// <param name="rack">The rack to be added</param>
        /// <param name="typeNode">The final node type (leave blank to get the opposite type of the current node)</param>
        public void AddTask(Racks rack, string typeNode = "")
        {
            while (rack.moving)
            {
                Thread.Sleep(1000); // Pause the program until the rack has arrived
            }

            // Create a new task
            Task newTask = new Task();
            newTask.firstDestination = rack.currentNode;
            newTask.finalDestination = World.grid.GetAvailableNode(rack.currentNode, typeNode); // Find a node where the rack can be placed
            newTask.getRack = rack;

            // Occupy the final destination so other robots won't go there
            newTask.firstDestination.occupied = false;
            newTask.finalDestination.occupied = true;
            
            Tasks.Add(newTask);
        }

        /// <summary>
        /// Adds the object rack to the list Racks.
        /// </summary>
        /// <param name="rack">Rack to be added</param>
        public void AddRack(Racks rack)
        {
            Racks.Add(rack);
        }

        /// <summary>
        /// Removes the rack from the list Racks.
        /// </summary>
        /// <param name="rack">Rack to be deleted</param>
        public void RemoveRack(Racks rack)
        {
            Racks.Remove(rack);
        }

        /// <summary>
        /// Adds a new product
        /// </summary>
        /// <param name="name">The name of the new product</param>
        public void AddProduct(string name)
        {
            // Create the product
            Product product = new Product();
            product.AddStock(product.maxStock);
            SortList(Products);

            int id = 0;
            if (Products.Count > 0)
            {
                id = Products.Last().id + 1; // Assign a new id
            }

            // Add the product and order the product
            product.AddProduct(id, name);
            Products.Add(product);
            AddOrderOrShipment(Orders, product);
        }

        /// <summary>
        /// Add the product to a given list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="product"></param>
        public void AddOrderOrShipment(List<Product> list, Product product)
        {
            list.Add(product);
        }

        /// <summary>
        /// Add stock to a product
        /// </summary>
        /// <param name="product"></param>
        /// <param name="cargo"></param>
        public void AddStock(Product product, int cargo)
        {
            if (SearchRackByProduct(product.name) != null)
            {
                product.AddStock(cargo);
            }
        }

        /// <summary>
        /// Remove stock of a product
        /// </summary>
        /// <param name="product"></param>
        /// <param name="cargo"></param>
        public void RemoveStock(Product product, int cargo)
        {
            foreach (Product p in Products)
            {
                if (p == product)
                {
                    p.RemoveStock(cargo);
                }
            }
        }

        /// <summary>
        /// Check if products need te be ordered
        /// </summary>
        public void CheckStock()
        {
            foreach (Product p in Products)
            {
                if (p.stock <= p.minStock)
                {
                    p.AddStock(p.maxStock);
                    orders.Add(p);
                }
            }
        }

        /// <summary>
        /// Search the rack that contains the given productname
        /// </summary>
        /// <param name="productName"></param>
        /// <returns>The rack that was found</returns>
        private Racks SearchRackByProduct(string productName)
        {
            Product result = SearchProduct(productName);
            if (result != null)
            {
                foreach (Racks rack in Racks)
                {
                    foreach (Product product in rack.contains)
                    {
                        if (product.name == productName)
                            return rack;
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Search product by productname
        /// </summary>
        /// <param name="productName"></param>
        /// <returns>Returns the product that was found</returns>
        private Product SearchProduct(string productName)
        {
            foreach (Product p in Products)
            {
                if (p.name == productName)
                {
                    return p;
                }
            }
            return null;
        }

        /// <summary>
        /// Sort a list by id
        /// </summary>
        /// <param name="list">The list to be ordered</param>
        private void SortList(List<Product> list)
        {
            int count = 0;
            foreach (Product p in list)
            {
                if (p.id != count)
                {
                    p.AlterProduct(count);
                }
                count++;
            }
        }
    }

    public class Product
    {
        private int Id;
        private string Name;
        private int Stock = 0;
        private int MaxStock = 10;
        private int MinStock = 0;

        /// <summary>
        /// Adds a new product
        /// </summary>
        /// <param name="id">Id of the product</param>
        /// <param name="name">Name of the product</param>
        /// <param name="maxStock">Maximal stocks</param>
        /// <param name="minStock">Minimal stocks</param>
        public void AddProduct(int id, string name, int maxStock = 10, int minStock = 0)
        {
            Id = id;
            Name = name;

            if (maxStock != 0)
                MaxStock = maxStock;

            if (MinStock >= 0)
                MinStock = minStock;
        }

        /// <summary>
        /// Change the product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="maxStock"></param>
        /// <param name="minStock"></param>
        public void AlterProduct(int id = -1, string name = null, int maxStock = -1, int minStock = -1)
        {
            if (id != -1)
                Id = id;

            if (name != null)
                Name = name;

            if (maxStock != -1)
                MaxStock = maxStock;

            if (minStock != -1)
                MinStock = minStock;
        }

        /// <summary>
        /// Add stock to a product
        /// </summary>
        /// <param name="cargo">Amount of stock to be added</param>
        public void AddStock(int cargo)
        {
            if (Stock + cargo <= MaxStock)
                Stock += cargo;
        }

        /// <summary>
        /// Remove stock from a product
        /// </summary>
        /// <param name="cargo">Amount of stock to be removed</param>
        public void RemoveStock(int cargo)
        {
            if (Stock - cargo >= 0)
                Stock -= cargo;
        }

        /// <summary>
        /// Clone a product
        /// </summary>
        /// <param name="product">Product to be cloned</param>
        /// <returns>Returns the clone of a product</returns>
        public Product Clone(Product product)
        {
            Product newProduct = new Product();
            newProduct.Id = product.id;
            newProduct.Name = product.name;
            newProduct.Stock = product.stock;

            return newProduct;
        }

        public int id { get { return Id; } }
        public string name { get { return Name; } }
        public int stock { get { return Stock; } }
        public int maxStock { get { return MaxStock; } }
        public int minStock { get { return MinStock; } }
    }
}