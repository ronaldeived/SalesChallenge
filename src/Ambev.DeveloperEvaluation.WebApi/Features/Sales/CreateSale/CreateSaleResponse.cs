namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;


/// <summary>
/// Represents the response model returned after creating a sale.
/// </summary>
public record CreateSaleResponse(
    Guid Id,
    string Number,
    DateTime Date,
    decimal Total
);