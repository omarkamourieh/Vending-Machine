using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VendingMachine.Domain;

namespace VendingMachine.Application.Commands
{
    public class ReturnCoinsCommand : IRequest<Dictionary<decimal, int>>
    {
        public class Handler : IRequestHandler<ReturnCoinsCommand, Dictionary<decimal, int>>
        {
            private readonly VendingMachine.Domain.VendingMachine _vendingMachine;
            public Handler(VendingMachine.Domain.VendingMachine vendingMachine)
            {
                _vendingMachine = vendingMachine;
            }

            public Task<Dictionary<decimal, int>> Handle(ReturnCoinsCommand request, CancellationToken cancellationToken)
            {
                var returnedCoins = _vendingMachine.ReturnCoins();
                return Task.FromResult(returnedCoins);
            }
        }
    }
}