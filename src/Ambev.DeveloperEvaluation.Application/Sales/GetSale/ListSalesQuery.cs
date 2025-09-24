using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public record ListSalesQuery() : IRequest<IEnumerable<SaleDto>>;