using MediatR;
using System.Collections.Generic;
using VendingMachine.Application.Queries;
using VendingMachine.Domain;

public class GetAvailableProductsQueryHandler : IRequestHandler<GetAvailableProductsQuery, List<Product>>
{
    private readonly VendingMachine.Domain.VendingMachine _vendingMachine;

    public GetAvailableProductsQueryHandler(VendingMachine.Domain.VendingMachine vendingMachine)
    {
        _vendingMachine = vendingMachine;
    }

    public Task<List<Product>> Handle(GetAvailableProductsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_vendingMachine.Products.ToList());
    }
}