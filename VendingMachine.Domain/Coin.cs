using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendingMachine.Domain
{
    public class Coin
    {
        public decimal Denomination { get; set; }
        public int Count { get; private set; }

        public Coin() { }

        public Coin(decimal denomination, int count)
        {
            Denomination = denomination;
            Count = count;
        }

        public void Add(int count)
        {
            if (count > 0) Count += count;
        }

        public void Take(int count)
        {
            if (count > 0) Count = (Count >= count) ? Count - count : 0;
        }
    }
}
