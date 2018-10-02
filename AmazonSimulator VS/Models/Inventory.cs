using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Inventory
    {
        private List<Product> Products = new List<Product>();

        public void PromptUser()
        {
            string productName;
            int amount;

            Console.WriteLine("Please enter your order.");
            Console.WriteLine("{product name} {amount}");

            string input = Console.ReadLine();
            string[] param = input.Split(' ');

            if (param.Count() == 2)
            {
                productName = param[0];
                int.TryParse(param[1], out amount);

                Product result = SearchProduct(productName);
                if (result != null)
                {
                    result.RemoveStock(amount);
                    Console.WriteLine("Ordered {0} of {1}", amount, productName);
                }
            }
            else
            {
                Console.WriteLine("Please try again.");
            }
        }

        public void AddProduct(string name)
        {
            Product product = new Product();
            SortList(Products);
            int id = 0;

            if (Products.Count > 0)
            {
                id = Products.Last().id + 1;
            }
            product.AddProduct(id, name);
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
                }
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
