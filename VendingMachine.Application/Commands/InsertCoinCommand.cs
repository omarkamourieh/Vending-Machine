using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace VendingMachine.Application.Commands
{
    public class InsertCoinCommand : IRequest
    {
        public decimal Denomination { get; }
        public InsertCoinCommand(decimal denomination)
        {
            Denomination = denomination;
        }


        public class Handler : IRequestHandler<InsertCoinCommand>
        {
            private readonly VendingMachine.Domain.VendingMachine _vendingMachine;

            public Handler(VendingMachine.Domain.VendingMachine vendingMachine)
            {
                _vendingMachine = vendingMachine;
            }

            public Task<Unit> Handle(InsertCoinCommand request, CancellationToken cancellationToken)
            {
                _vendingMachine.InsertCoin(request.Denomination);
                return Task.FromResult(Unit.Value);
            }
        }
    }
}
