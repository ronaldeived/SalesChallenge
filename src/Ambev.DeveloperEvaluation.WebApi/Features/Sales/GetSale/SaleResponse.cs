using System.Runtime.InteropServices.JavaScript;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// Represents the detailed response model for a sale.
/// </summary>
public record SaleResponse(
    Guid Id,
    string Number,
    DateTime Date,
    Guid CustomerId,
    string CustomerName,
    Guid BranchId,
    string BranchName,
    bool IsCancelled,
    decimal Total,
    IEnumerable<SaleItemResponse> Items);


/// <summary>
/// Represents the response model for a sale item.
/// </summary>
public record SaleItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountPercent,
    decimal Total
);