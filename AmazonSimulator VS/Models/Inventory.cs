using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Models
{
    public class Inventory
    {
        private List<Product> Products = new List<Product>();
        private List<Product> Orders = new List<Product>();
        private List<Product> Shipments = new List<Product>();
        private List<Racks> Racks = new List<Racks>();
        private World World;

        public List<Task> Tasks = new List<Task>();

        public List<Product> orders { get { return Orders; } }
        public List<Product> shipments { get { return Shipments; } }
        public List<Racks> racks { get { return Racks; } }

        /// <summary>
        /// Sets the object world.
        /// </summary>
        /// <param name="world">World</param>
        public Inventory(World world)
        {
            World = world;
        }

        /// <summary>
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        /// <param name="world"></param>
        public void PromptUser(World world)
        {
            this.World = world;
            string productName = "";
            int amount = 0;

            Console.WriteLine("Please enter your order.");
            Console.WriteLine("{product name} {amount}");

            string input = Console.ReadLine();
            string[] param = input.Split(' ');

            if (param.Count() <= 2)
            {
                productName = param[0];

                Product result = SearchProduct(productName);
                if (result != null && param.Count() == 2)
                {
                    int.TryParse(param[1], out amount);
                    Racks rack = SearchRackByProduct(productName);
                    if (rack != null)
                    {
                        if (amount <= result.stock && amount > 0)
                        {
                            // Create a copy of the product to be shipped
                            Product productToShip = new Product();
                            productToShip = productToShip.Clone(result);

                            productToShip.RemoveStock(productToShip.stock - amount);
                            //result.RemoveStock(amount);

                            AddOrderOrShipment(Shipments, productToShip);

                            Thread inventoryThread = new Thread(() => AddTask(rack, "cargoNode"));
                            inventoryThread.Start();
                        }
                        else
                        {
                            Console.WriteLine("Your order is too large or too small.");
                        }
                        //result.RemoveStock(amount);
                        Console.WriteLine("Ordered {0} of {1}", amount, productName);
                    }
                    else
                    {
                        Console.WriteLine("Rack couldn't be found, try again later.");
                    }
                    //AddTask(SearchRackByProduct(productName));

                }
                else if (param.Count() == 1 && productName == "list")
                {
                    ShowStock();
                }
                else if (param.Count() == 1)
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
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        /// <param name="rack"></param>
        public void AddTask(Racks rack, string typeNode = "")
        {
            while (rack.moving)
            {
                Thread.Sleep(1000);
            }
            Task newTask = new Task();
            newTask.firstDestination = rack.currentNode;
            newTask.finalDestination = World.grid.GetAvailableNode(rack.currentNode, typeNode);
            newTask.firstDestination.occupied = false;
            newTask.finalDestination.occupied = true;
            newTask.getRack = rack;
            Tasks.Add(newTask);
        }

        /// <summary>
        /// Adds the object rack to the list Racks.
        /// </summary>
        /// <param name="rack">Racks</param>
        public void AddRack(Racks rack)
        {
            Racks.Add(rack);
        }

        /// <summary>
        /// Removes the rack from the list Racks.
        /// </summary>
        /// <param name="rack">Racks</param>
        public void RemoveRack(Racks rack)
        {
            Racks.Remove(rack);
        }

        /// <summary>
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        /// <param name="name"></param>
        public void AddProduct(string name)
        {
            Product product = new Product();
            product.AddStock(product.maxStock);
            SortList(Products);
            int id = 0;

            if (Products.Count > 0)
            {
                id = Products.Last().id + 1;
            }
            product.AddProduct(id, name);
            Products.Add(product);
            AddOrderOrShipment(Orders, product);
            //Orders.Add(product);
        }

        /// <summary>
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        /// <param name="list"></param>
        /// <param name="product"></param>
        public void AddOrderOrShipment(List<Product> list, Product product)
        {
            list.Add(product);
        }

        /// <summary>
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Product RetrieveProduct(Product product)
        {
            foreach (Product p in Products)
            {
                if (p == product)
                    return p;
            }
            Console.WriteLine("{0} couldn't be found", product.name);
            return null;
        }

        /// <summary>
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        /// <param name="product"></param>
        /// <param name="cargo"></param>
        public void AddStock(Product product, int cargo)
        {
            if (RetrieveProduct(product) != null)
            {
                product.AddStock(cargo);
            }
        }

        /// <summary>
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
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
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
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
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
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
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
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
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        /// <param name="list"></param>
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
        /// Sets the given variables.
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
        ///<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
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

        public void AddStock(int cargo)
        {
            if (Stock + cargo <= MaxStock)
                Stock += cargo;
        }

        public void RemoveStock(int cargo)
        {
            if (Stock - cargo >= 0)
                Stock -= cargo;
        }

        /// <summary>
        /// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<JESSE
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
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