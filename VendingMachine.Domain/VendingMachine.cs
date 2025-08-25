using System;
using System.Collections.Generic;
using System.Linq;

namespace VendingMachine.Domain
{
    public class VendingMachine
    {
        public List<Product> Products { get; }
        public Wallet Wallet { get; }
        public decimal InsertedAmount { get; private set; }

        public VendingMachine(IEnumerable<Product> products, Wallet wallet)
        {
            Products = products.ToList();
            Wallet = wallet;
            InsertedAmount = 0m;
        }

        public void InsertCoin(decimal denomination)
        {
            Wallet.AddCoin(denomination, 1);
            InsertedAmount += denomination;
        }

        public Dictionary<decimal, int>? ReturnCoins()
        {
            if (InsertedAmount == 0)
            {
                return null;
            }

            var change = Wallet.GetChange(InsertedAmount);
            if (change != null)
            {
                Wallet.TryTakeChange(InsertedAmount, out _);
                InsertedAmount = 0m;
            }
            return change;
        }

        public (bool Success, string Message, Dictionary<decimal, int>? Change) BuyProduct(string productName)
        {
            var product = GetProductByName(productName);

            if (product == null)
                return (false, "Product not found.", null);

            if (product.Quantity <= 0)
                return (false, "Product out of stock.", null);

            if (InsertedAmount < product.Price)
                return (false, "Insufficient amount.", null);

            decimal changeAmount = InsertedAmount - product.Price;
            Dictionary<decimal, int>? change = null;

            if (changeAmount > 0)
            {
                change = Wallet.GetChange(changeAmount);
                if (change == null)
                    return (false, "Cannot return change with available coins.", null);
            }

            product.TryDispense();

            if (changeAmount > 0)
                Wallet.TryTakeChange(changeAmount, out _);

            InsertedAmount = 0m;
            return (true, $"Product {product.Name} purchased. Thank you!", change);
        }

        public Product? GetProductByName(string productName)
        {
            return Products.FirstOrDefault(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
        }
        public (bool Success, string Message) RestockProduct(string productName, int quantityToAdd)
        {
            if (quantityToAdd <= 0)
                return (false, "Quantity to add must be positive.");

            var product = GetProductByName(productName);
            if (product == null)
                return (false, $"Product '{productName}' not found.");

            product.Restock(quantityToAdd);
            return (true, $"SUCCESS: Restocked {productName} with {quantityToAdd} items. New quantity: {product.Quantity}.");
        }
        public decimal WithdrawMoney()
        {
            decimal amount = Wallet.GetTotalAmount();
            Wallet.Empty();
            return amount;
        }
        public (bool Success, string Message) AddProduct(string name, decimal price, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name) || price <= 0 || quantity < 0)
                return (false, "Invalid product details provided.");

            if (Products.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                return (false, $"A product with the name '{name}' already exists.");

            Products.Add(new Product(name, price, quantity));
            return (true, $"SUCCESS: Product '{name}' added.");
        }
    }
}