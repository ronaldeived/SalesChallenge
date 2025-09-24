using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleCommandHandler(ISaleRepository saleRepository) : IRequestHandler<DeleteSaleCommand, bool>
{
    public async Task<bool> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        await saleRepository.DeleteAsync(request.Id, cancellationToken);
        
        return await saleRepository.GetByIdAsync(request.Id, cancellationToken) is null; 
    }
}