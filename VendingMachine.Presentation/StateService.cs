using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using VendingMachine.Domain;

namespace VendingMachine.Presentation
{
    public class StateService
    {
        private const string StateFilePath = "vending_machine_state.json";

        public void SaveState(Domain.VendingMachine machine)
        {
            var state = new VendingMachineState { Products = machine.Products, Wallet = machine.Wallet };
            var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
            var json = JsonSerializer.Serialize(state, options);
            File.WriteAllText(StateFilePath, json);
        }

        public Domain.VendingMachine LoadState()
        {
            if (!File.Exists(StateFilePath))
            {
                var initialMachine = CreateDefaultMachine();
                SaveState(initialMachine);
                return initialMachine;
            }

            var json = File.ReadAllText(StateFilePath);
            if (string.IsNullOrWhiteSpace(json)) 
            {
                var initialMachine = CreateDefaultMachine();
                SaveState(initialMachine);
                return initialMachine;
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var state = JsonSerializer.Deserialize<VendingMachineState>(json, options);

            if (state == null || state.Products == null || state.Wallet == null)
            {
                System.Console.WriteLine("WARNING: State file was corrupt. Loading default state.");
                var defaultMachine = CreateDefaultMachine();
                SaveState(defaultMachine);
                return defaultMachine;
            }

            return new Domain.VendingMachine(state.Products, state.Wallet);
        }

        private class VendingMachineState
        {
            public List<Product> Products { get; set; } = new();
            public Wallet Wallet { get; set; } = new(new List<Coin>());
        }

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