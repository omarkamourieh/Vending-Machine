using System;
using System.Collections.Generic;
using System.Linq;

namespace VendingMachine.Domain
{
    public class Wallet
    {
        public Dictionary<decimal, Coin> Coins { get; set; }

        public Wallet()
        {
            Coins = new Dictionary<decimal, Coin>();
        }

        public Wallet(IEnumerable<Coin> initialCoins)
        {
            Coins = initialCoins.ToDictionary(c => c.Denomination, c => new Coin(c.Denomination, c.Count));
        }

        public void AddCoin(decimal denomination, int count = 1)
        {
            if (Coins.TryGetValue(denomination, out var coin))
            {
                coin.Add(count);
            }
            else
            {
                Coins[denomination] = new Coin(denomination, count);
            }
        }

        public Dictionary<decimal, int>? GetChange(decimal amount)
        {
            var result = new Dictionary<decimal, int>();
            var sortedCoins = Coins.Values.OrderByDescending(c => c.Denomination).ToList();
            decimal remaining = amount;

            foreach (var coin in sortedCoins)
            {
                if (coin.Denomination <= 0) continue;

                int needed = (int)(remaining / coin.Denomination);
                int toUse = Math.Min(needed, coin.Count);

                if (toUse > 0)
                {
                    result[coin.Denomination] = toUse;
                    remaining -= toUse * coin.Denomination;
                }
            }

            if (remaining < 0.001m)
                return result;

            return null;
        }

        public bool TryTakeChange(decimal amount, out Dictionary<decimal, int>? change)
        {
            change = GetChange(amount);
            if (change == null)
                return false;

            foreach (var (denomination, count) in change)
            {
                if (Coins.TryGetValue(denomination, out var coin))
                {
                    coin.Take(count);
                }
            }
            return true;
        }

        public decimal GetTotalAmount()
        {
            return Coins.Values.Sum(c => c.Denomination * c.Count);
        }

        public void Empty()
        {
            foreach (var coin in Coins.Values)
            {
                coin.Take(coin.Count);
            }
        }

        public Dictionary<decimal, int> GetState()
        {
            return Coins.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Count);
        }
    }
}