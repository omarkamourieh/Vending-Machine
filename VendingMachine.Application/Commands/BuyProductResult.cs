using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendingMachine.Application.Commands
{
    public class BuyProductResult
    {
        public string Message { get; set; }  = string.Empty;
        public Dictionary<decimal, int>? Change { get; set; }
    }
}
