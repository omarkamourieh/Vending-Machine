using MediatR;
using VendingMachine.Domain;

namespace VendingMachine.Application.Commands
{
    public class AddProductCommand : IRequest<(bool Success, string Message)>
    {
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public class Handler : IRequestHandler<AddProductCommand, (bool Success, string Message)>
        {
            private readonly VendingMachine.Domain.VendingMachine _vendingMachine;

            public Handler(VendingMachine.Domain.VendingMachine vendingMachine)
            {
                _vendingMachine = vendingMachine;
            }

            public Task<(bool Success, string Message)> Handle(AddProductCommand request, CancellationToken cancellationToken)
            {
                var result = _vendingMachine.AddProduct(request.Name, request.Price, request.Quantity);
                return Task.FromResult(result);
            }
        }
    }
}