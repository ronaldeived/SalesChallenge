using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public record UpdateSaleCommand(
    Guid Id,
    string Number,
    DateTime Date,
    Guid CustomerId,
    string CustomerName,
    Guid BranchId,
    string BranchName,
    IEnumerable<UpdateSaleItemInput> Items
) : IRequest<SaleDto?>;

public record UpdateSaleItemInput(
    Guid Id, 
    Guid ProductId, 
    string ProductName, 
    int Quantity, 
    decimal UnitPrice);