using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public record CreateSaleCommand(
    string Number,
    DateTime Date,
    Guid CustomerId,
    string CustomerName,
    Guid BranchId,
    string BranchName,
    IEnumerable<CreateSaleItemInput> Items
) : IRequest<SaleDto>;

public record CreateSaleItemInput(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);