using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using VendingMachine.Domain;

namespace VendingMachine.Application.Queries
{
    public class GetWalletStateQuery : IRequest<Dictionary<decimal, int>> { }

    public class GetWalletStateQueryHandler : IRequestHandler<GetWalletStateQuery, Dictionary<decimal, int>>
    {
        private readonly VendingMachine.Domain.VendingMachine _vendingMachine;

        public GetWalletStateQueryHandler(VendingMachine.Domain.VendingMachine vendingMachine)
        {
            _vendingMachine = vendingMachine;
        }

        public Task<Dictionary<decimal, int>> Handle(GetWalletStateQuery request, CancellationToken cancellationToken)
        {
            var walletState = _vendingMachine.Wallet.GetState();

            return Task.FromResult(walletState);
        }
    }
}