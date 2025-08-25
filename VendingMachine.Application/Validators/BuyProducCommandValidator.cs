using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using VendingMachine.Application.Commands;
using VendingMachine.Domain;

namespace VendingMachine.Application.Validators
{
    public class BuyProductCommandValidator : AbstractValidator<BuyProductCommand>
    {
        private readonly VendingMachine.Domain.VendingMachine _vendingMachine;

        public BuyProductCommandValidator(VendingMachine.Domain.VendingMachine vendingMachine)
        {
            _vendingMachine = vendingMachine;

            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name cannot be empty.")
                .MustAsync(ProductMustExistAsync)
                .WithMessage(cmd => $"The product '{cmd.ProductName}' is not available in this machine.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.ProductName)
                        .MustAsync(ProductMustBeInStockAsync)
                        .WithMessage(cmd => $"Sorry, '{cmd.ProductName}' is out of stock.");
                });
        }

        private async Task<bool> ProductMustExistAsync(string productName, CancellationToken cancellationToken)
        {
            var product = _vendingMachine.GetProductByName(productName);
            return await Task.FromResult(product != null);
        }

        private async Task<bool> ProductMustBeInStockAsync(string productName, CancellationToken cancellationToken)
        {
            var product = _vendingMachine.GetProductByName(productName);
            return await Task.FromResult(product!.Quantity > 0);
        }
    }
}