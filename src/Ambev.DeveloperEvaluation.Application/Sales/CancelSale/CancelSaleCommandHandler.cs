using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleCommandHandler(ISaleRepository saleRepository, IMediator mediator) 
    : IRequestHandler<CancelSaleCommand, SaleDto?>
{
    public async Task<SaleDto?> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
            throw new ArgumentException("Sale Id must be provided.");

        var sale = await saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale is null) return null;

        try
        {
            sale.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            throw new ApplicationException("Unable to cancel the sale.", ex);
        }

        await saleRepository.UpdateAsync(sale, cancellationToken);

        return new SaleDto(
            sale.Id,
            sale.Number,
            sale.Date,
            sale.CustomerId,
            sale.CustomerName,
            sale.BranchId,
            sale.BranchName,
            sale.IsCancelled,
            sale.Total,
            sale.Items.Select(i =>
                new SaleItemDto(i.Id, i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.DiscountPercent, i.Total))
        );
    }
}
