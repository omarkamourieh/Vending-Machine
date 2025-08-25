using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendingMachine.Domain;

namespace VendingMachine.Application.Queries
{
    public class GetAvailableProductsQuery : IRequest<List<Product>>
    {
    }
}
