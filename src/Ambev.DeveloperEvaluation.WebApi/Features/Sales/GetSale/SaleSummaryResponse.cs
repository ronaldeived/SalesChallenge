namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// Represents a summarized response model for listing sales.
/// </summary>
public record SaleSummaryResponse(
    Guid Id,
    string Number,
    DateTime Date,
    string CustomerName,
    string BranchName,
    bool IsCancelled,
    decimal Total
);