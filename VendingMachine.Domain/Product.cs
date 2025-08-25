using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendingMachine.Domain
{
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; private set; }

        public Product()
        {
            Name = string.Empty;
        }

        public Product(string name, decimal price, int quantity)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        public bool TryDispense()
        {
            if (Quantity <= 0)
            {
             return false;
            }
            Quantity--;
            return true;
        }
        public void Restock(int quantityToAdd)
        {
            if (quantityToAdd > 0)
            {
                Quantity += quantityToAdd;
            }
        }
    }
}
