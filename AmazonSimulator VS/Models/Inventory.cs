using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Inventory
    {
        private List<Product> Products = new List<Product>();
        private List<Product> Orders = new List<Product>();
        private List<Racks> Racks = new List<Racks>();
        private World World;

        public List<Task> Tasks = new List<Task>();

        public List<Product> orders { get { return Orders; } }

        public Inventory(World world)
        {
            World = world;
        }

        public void PromptUser(World world)
        {
            this.World = world;
            string productName;
            int amount;

            Console.WriteLine("Please enter your order.");
            Console.WriteLine("{product name} {amount}");

            string input = Console.ReadLine();
            string[] param = input.Split(' ');

            if (param.Count() <= 2)
            {
                productName = param[0];
                

                Product result = SearchProduct(productName);
                if (result != null && param[1] != null)
                {
                    int.TryParse(param[1], out amount);
                    result.RemoveStock(amount);
                    Console.WriteLine("Ordered {0} of {1}", amount, productName);
                }
                else
                {
                    Console.WriteLine("{0}, was not found. Do you want to add this item to the invetory? (Y/N)", productName);
                    string AddProduct = Console.ReadLine();
                    bool correct = false;
                    while (!correct)
                    {
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
            CheckStock();
        }

        public void AddTask(Racks rack)
        {
            Task newTask = new Task();
            newTask.firstDestination = rack.currentNode;
            newTask.finalDestination = GetAvailableNode(rack.currentNode);
            newTask.getRack = rack;
            Tasks.Add(newTask);
        }

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
            Orders.Add(product);
        }

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

        public void AddStock(Product product, int cargo)
        {
            if (RetrieveProduct(product) != null)
            {
                product.AddStock(cargo);
            }
        }

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

        public void CheckStock()
        {
            foreach (Product p in Products)
            {
                if (p.stock < p.minStock)
                {
                    p.AddStock(p.maxStock - p.stock);
                    orders.Add(p);
                }
            }
        }

        //private void MakeTask(string productName)
        //{
        //    Task task = new Task();
        //    Racks rack = SearchRackByProduct(productName);
        //    if (rack != null)
        //    {
        //        Node availableNode = GetAvailableNode(rack.currentNode);
        //        if (availableNode != null)
        //        {
        //            task.firstDestination = rack.currentNode;
        //            task.finalDestination = availableNode;
        //            task.getRack = rack;
        //            Tasks.Add(task);
        //        }
        //    }
        //}

        private Node GetAvailableNode(Node rackNode)
        {
            List<Node> getNodes = World.grid.GetNodes;
            //Node emptyNode = null;
            string checkNode = (rackNode.type == "cargoNode") ? "storageNode" : "cargoNode";

            for (int i = 0; i < getNodes.Count - 1; i++)
            {
                if (getNodes[i].type == checkNode && getNodes[i].taken == false)
                    for (int j = 0; j <= Racks.Count; j++)
                    {
                        if (Racks.Count == 0)
                        {
                            getNodes[i].taken = true; 
                            return getNodes[i];
                        }

                        if (getNodes[i] != Racks[j].currentNode || Racks.Count == 0)
                        {
                            getNodes[i].taken = true;
                            return getNodes[i];
                        }
                    }
            }
            return null;
        }

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
        private int MinStock = 5;

        public void AddProduct(int id, string name, int maxStock = 0, int minStock = 0)
        {
            Id = id;
            Name = name;

            if (maxStock != 0)
                MaxStock = maxStock;

            if (MinStock != 0)
                MinStock = minStock;
        }

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

        public int id { get { return Id; } }
        public string name { get { return Name; } }
        public int stock { get { return Stock; } }
        public int maxStock { get { return MaxStock; } }
        public int minStock { get { return MinStock; } }
    }
}
