using MediatR;
using VendingMachine.Domain;
using System.Collections.Generic;

namespace VendingMachine.Application.Queries
{
    public class GetMachineStatusQuery : IRequest<MachineStatusResult> { }

    public class MachineStatusResult
    {
        public List<Product> Products { get; set; } = new();
        public Dictionary<decimal, int> WalletState { get; set; } = new();
        public decimal TotalMoney { get; set; }
    }

    public class GetMachineStatusQueryHandler : IRequestHandler<GetMachineStatusQuery, MachineStatusResult>
    {
        private readonly VendingMachine.Domain.VendingMachine _machine;
        public GetMachineStatusQueryHandler(VendingMachine.Domain.VendingMachine machine) => _machine = machine;

        public Task<MachineStatusResult> Handle(GetMachineStatusQuery request, CancellationToken token)
        {
            var result = new MachineStatusResult
            {
                Products = _machine.Products,
                WalletState = _machine.Wallet.GetState(),
                TotalMoney = _machine.Wallet.GetTotalAmount()
            };
            return Task.FromResult(result);
        }
    }
}