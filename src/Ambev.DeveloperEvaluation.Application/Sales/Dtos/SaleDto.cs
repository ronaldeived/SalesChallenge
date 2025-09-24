namespace Ambev.DeveloperEvaluation.Application.Sales.Dtos;

public record SaleDto(
    Guid Id, 
    string Number, 
    DateTime Date, 
    Guid CustomerId, 
    string CustomerName, 
    Guid BranchId, 
    string BranchName, 
    bool IsCancelled, 
    decimal Total, 
    IEnumerable<SaleItemDto> Items);
    
public record SaleItemDto(
    Guid Id, 
    Guid ProductId, 
    string ProductName, 
    int Quantity, 
    decimal UnitPrice, 
    decimal DiscountPercent, 
    decimal Total);