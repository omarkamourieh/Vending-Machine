using MediatR;
using VendingMachine.Domain;

namespace VendingMachine.Application.Commands
{
    public class RestockProductCommand : IRequest<(bool Success, string Message)>
    {
        public string ProductName { get; set; }
        public int QuantityToAdd { get; set; }

        public class Handler : IRequestHandler<RestockProductCommand, (bool Success, string Message)>
        {
            private readonly VendingMachine.Domain.VendingMachine _machine;
            public Handler(VendingMachine.Domain.VendingMachine machine) => _machine = machine;

            public Task<(bool Success, string Message)> Handle(RestockProductCommand request, CancellationToken token)
            {
                var result = _machine.RestockProduct(request.ProductName, request.QuantityToAdd);
                return Task.FromResult(result);
            }
        }
    }
}