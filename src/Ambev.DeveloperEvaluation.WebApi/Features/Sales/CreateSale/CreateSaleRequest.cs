namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public record CreateSaleRequest(
    string Number,
    DateTime Date,
    Guid CustomerId,
    string CustomerName,
    Guid BranchId,
    string BranchName,
    IEnumerable<CreateSaleItemRequest> Items
);

public record CreateSaleItemRequest(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);