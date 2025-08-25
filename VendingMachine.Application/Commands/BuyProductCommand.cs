using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace VendingMachine.Application.Commands
{
    public class BuyProductCommand : IRequest<BuyProductResult>
    {
        public string ProductName { get; }
        public BuyProductCommand(string productName)
        {
            ProductName = productName;
        }

        public class Handler : IRequestHandler<BuyProductCommand, BuyProductResult>
        {
            private readonly VendingMachine.Domain.VendingMachine _vendingMachine;

            public Handler(VendingMachine.Domain.VendingMachine vendingMachine)
            {
                _vendingMachine = vendingMachine;
            }

            public Task<BuyProductResult> Handle(BuyProductCommand request, CancellationToken cancellationToken)
            {
                var result = _vendingMachine.BuyProduct(request.ProductName);

                return Task.FromResult(new BuyProductResult
                {
                    Message = result.Message,
                    Change = result.Change
                });
            }
        }
    }
}
