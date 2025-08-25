using Serilog;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using VendingMachine.Application.Commands;
using VendingMachine.Application.Common.Behaviors;
using VendingMachine.Application.Queries;
using VendingMachine.Presentation;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting Vending Machine application in stateless mode.");

    var stateService = new StateService();
    var vendingMachine = stateService.CreateDefaultMachine();

    var services = new ServiceCollection();
    services.AddLogging(builder => builder.AddSerilog(dispose: true));
    services.AddSingleton(vendingMachine);
    services.AddMediatR(typeof(BuyProductCommand).Assembly);
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    services.AddValidatorsFromAssembly(typeof(BuyProductCommand).Assembly);

    var provider = services.BuildServiceProvider();
    var mediator = provider.GetRequiredService<IMediator>();

    const string AdminCode = "admin123";
    bool isAdminMode = false;

    Console.WriteLine("Application Started. Vending Machine is ready.");

    while (true)
    {
        Console.WriteLine("\n----------------------------------------");

        if (isAdminMode)
        {
            Console.WriteLine("--- ADMIN MODE ---");
            Console.WriteLine("1. Show Machine Status");
            Console.WriteLine("2. Add New Product");
            Console.WriteLine("3. Restock Product");
            Console.WriteLine("4. Withdraw All Money");
            Console.WriteLine("0. Exit Admin Mode");
        }
        else
        {
            Console.WriteLine("--- Vending Machine ---");
            Console.WriteLine($"Inserted amount: {vendingMachine.InsertedAmount:C}");
            Console.WriteLine("1. Show Products");
            Console.WriteLine("2. Insert Coin");
            Console.WriteLine("3. Buy Product");
            Console.WriteLine("4. Return Coins");
            Console.WriteLine("0. Exit Application");
        }

        Console.Write("\nChoose an option: ");
        var input = Console.ReadLine();

        if (!isAdminMode && input == AdminCode)
        {
            isAdminMode = true;
            Log.Information("Admin mode activated.");
            continue;
        }

        if (isAdminMode)
        {
            switch (input)
            {
                case "1":
                    var status = await mediator.Send(new GetMachineStatusQuery());
                    Console.WriteLine("\n--- MACHINE STATUS ---");
                    status.Products.ForEach(p => Console.WriteLine($"- {p.Name} - {p.Price:C} ({p.Quantity} left)"));
                    Console.WriteLine("\nWALLET:");
                    status.WalletState.ToList().ForEach(c => Console.WriteLine($"- {c.Value} x {c.Key:C}"));
                    Console.WriteLine($"\nTotal Money In Wallet: {status.TotalMoney:C}");
                    break;
                case "2":
                    try
                    {
                        Console.Write("Enter new product name: ");
                        var newName = Console.ReadLine();
                        Console.Write("Enter price: ");
                        if (!decimal.TryParse(Console.ReadLine(), out var newPrice)) { Console.WriteLine("Invalid price format."); break; }
                        Console.Write("Enter quantity: ");
                        if (!int.TryParse(Console.ReadLine(), out var newQuantity)) { Console.WriteLine("Invalid quantity format."); break; }

                        var command = new AddProductCommand { Name = newName, Price = newPrice, Quantity = newQuantity };
                        var result = await mediator.Send(command);
                        Console.WriteLine(result.Message);
                        if (result.Success) Log.Information("Admin action: {Message}", result.Message);
                    }
                    catch (ValidationException ex) { Log.Warning("Validation failed for AddProductCommand: {@Errors}", ex.Errors.Select(e => e.ErrorMessage)); Console.WriteLine($"ERROR: {ex.Errors.First().ErrorMessage}"); }
                    break;
                case "3":
                    Console.Write("Enter product name to restock: ");
                    var name = Console.ReadLine();
                    Console.Write("Enter quantity to add: ");
                    if (!int.TryParse(Console.ReadLine(), out var qty) || qty <= 0) { Console.WriteLine("Invalid quantity."); break; }
                    var restockResult = await mediator.Send(new RestockProductCommand { ProductName = name, QuantityToAdd = qty });
                    Console.WriteLine(restockResult.Message);
                    if (restockResult.Success) Log.Information("Admin action: {Message}", restockResult.Message); 
                    break;
                case "4":
                    var amount = await mediator.Send(new WithdrawMoneyCommand());
                    Console.WriteLine($"SUCCESS: Withdrew {amount:C} from the machine.");
                    Log.Information("Admin withdrew {Amount:C} from the machine.", amount);
                    break;
                case "0":
                    isAdminMode = false;
                    Log.Information("Exited admin mode.");
                    break;
                default:
                    Console.WriteLine("Invalid admin option.");
                    break;
            }
        }
        else
        {
            switch (input)
            {
                case "1":
                    var productsList = await mediator.Send(new GetAvailableProductsQuery());
                    Console.WriteLine("\nProducts:");
                    productsList.ForEach(p => Console.WriteLine($"{p.Name} - {p.Price:C} ({p.Quantity} left)"));
                    break;
                case "2":
                    Console.Write("Enter coin (0.10, 0.20, 0.50, 1.00): ");
                    if (decimal.TryParse(Console.ReadLine(), out var coin))
                    {
                        var validCoins = new[] { 0.10m, 0.20m, 0.50m, 1.00m };
                        if (validCoins.Contains(coin))
                        {
                            await mediator.Send(new InsertCoinCommand(coin));
                            Log.Information("Customer inserted coin: {CoinValue:C}", coin);
                        }
                        else { Console.WriteLine("Invalid coin denomination."); }
                    }
                    else { Console.WriteLine("Invalid input."); }
                    break;
                case "3":
                    Console.Write("Enter product name: ");
                    var productName = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(productName)) break;
                    try
                    {
                        var result = await mediator.Send(new BuyProductCommand(productName));
                        Console.WriteLine(result.Message);
                        if (result.Change?.Count > 0)
                        {
                            Console.WriteLine("Change returned:");
                            result.Change.ToList().ForEach(c => Console.WriteLine($"{c.Value} x {c.Key:C}"));
                        }
                        if (result.Message != null && result.Message.Contains("purchased"))
                        {
                            Log.Information("Product {ProductName} was successfully purchased.", productName);
                        }
                    }
                    catch (ValidationException ex) { Log.Warning("Validation failed for purchase: {@Errors}", ex.Errors.Select(e => e.ErrorMessage)); Console.WriteLine($"ERROR: {ex.Errors.First().ErrorMessage}"); }
                    break;
                case "4":
                    var returned = await mediator.Send(new ReturnCoinsCommand());
                    if (returned?.Count > 0)
                    {
                        Console.WriteLine("Coins returned:");
                        returned.ToList().ForEach(c => Console.WriteLine($"{c.Value} x {c.Key:C}"));
                        Log.Information("Returned {CoinCount} coins to the customer.", returned.Values.Sum());
                    }
                    else { Console.WriteLine("No coins to return."); }
                    break;
                case "0":
                    Log.Information("Exiting application.");
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}