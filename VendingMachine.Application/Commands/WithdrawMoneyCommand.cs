using MediatR;
using VendingMachine.Domain;

namespace VendingMachine.Application.Commands
{
    public class WithdrawMoneyCommand : IRequest<decimal>
    {
        public class Handler : IRequestHandler<WithdrawMoneyCommand, decimal>
        {
            private readonly VendingMachine.Domain.VendingMachine _machine;
            public Handler(VendingMachine.Domain.VendingMachine machine) => _machine = machine;

            public Task<decimal> Handle(WithdrawMoneyCommand request, CancellationToken token)
            {
                var amount = _machine.WithdrawMoney();
                return Task.FromResult(amount);
            }
        }
    }
}