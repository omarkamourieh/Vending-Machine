using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using VendingMachine.Domain;

namespace VendingMachine.Presentation
{
    public class StateService
    {
        public Domain.VendingMachine CreateDefaultMachine()
        {
            var products = new List<Product>
            {
                new Product("Tea", 1.30m, 10),
                new Product("Espresso", 1.80m, 20),
                new Product("Juice", 1.80m, 20),
                new Product("Energy Bar", 1.89m, 15),
            };
            var coins = new List<Coin>
            {
                new Coin(0.10m, 100), 
                new Coin(0.20m, 100),
                new Coin(0.50m, 100), 
                new Coin(1.00m, 100)
            };
            var wallet = new Wallet(coins);
            return new Domain.VendingMachine(products, wallet);
        }
    }
}