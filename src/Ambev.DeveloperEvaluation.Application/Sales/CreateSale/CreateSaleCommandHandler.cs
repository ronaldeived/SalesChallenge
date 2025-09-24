using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleCommandHandler(ISaleRepository saleRepository)
    : IRequestHandler<CreateSaleCommand, SaleDto>
{
    public async Task<SaleDto> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = Sale.Create(
            request.Number,
            request.Date,
            request.CustomerId,
            request.CustomerName,
            request.BranchId,
            request.BranchName);

        foreach (var item in request.Items)
        {
            var saleItem = new SaleItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);
            sale.AddItem(saleItem);
        }

        await saleRepository.AddAsync(sale, cancellationToken);

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