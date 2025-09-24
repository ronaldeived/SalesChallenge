namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public record UpdateSaleRequest(
    Guid Id,
    string Number,
    DateTime Date,
    Guid CustomerId,
    string CustomerName,
    Guid BranchId,
    string BranchName,
    IEnumerable<UpdateSaleItemRequest> Items
);

public record UpdateSaleItemRequest(Guid Id, Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);