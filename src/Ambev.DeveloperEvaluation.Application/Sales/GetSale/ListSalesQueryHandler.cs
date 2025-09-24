using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class ListSalesQueryHandler(ISaleRepository saleRepository)
    : IRequestHandler<ListSalesQuery, IEnumerable<SaleDto>>
{
    public async Task<IEnumerable<SaleDto>> Handle(ListSalesQuery request, CancellationToken cancellationToken)
    {
        var sales = await saleRepository.ListAsync(cancellationToken);

        return sales.Select(sale => new SaleDto(
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
        ));
    }
}