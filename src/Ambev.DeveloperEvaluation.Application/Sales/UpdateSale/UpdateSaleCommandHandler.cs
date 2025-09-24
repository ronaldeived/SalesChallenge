using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleCommandHandler(ISaleRepository saleRepository) : IRequestHandler<UpdateSaleCommand, SaleDto?>
{
    public async Task<SaleDto?> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await saleRepository.GetByIdAsNoTrackingAsync(request.Id, cancellationToken);
        if (sale is null) return null;
        
        sale.UpdateHeader(
            request.Number,
            request.Date,
            request.CustomerId,
            request.CustomerName,
            request.BranchId,
            request.BranchName
        );

        var newItems = request.Items.Select(i =>
            new SaleItem(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)
        ).ToList();

        sale.ReplaceItems(newItems);

        await saleRepository.UpdateWithItemsAsync(sale, cancellationToken);

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
            sale.Items.Select(i => new SaleItemDto(i.Id, i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.DiscountPercent, i.Total))
        );
    }
}