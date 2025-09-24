using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// Profile for mapping between Application and API GetSale feature.
/// </summary>
public class GetSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetSale feature.
    /// </summary>
    public GetSaleProfile()
    {
        CreateMap<Guid, GetSaleByIdQuery>()
            .ConstructUsing(id => new GetSaleByIdQuery(id));
        CreateMap<SaleDto, SaleResponse>();
        CreateMap<SaleItemDto, SaleItemResponse>();
        CreateMap<SaleDto, SaleSummaryResponse>();
    }
}