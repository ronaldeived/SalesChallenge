using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.Number).NotEmpty().WithMessage("Sale number is required.");
        RuleFor(x => x.Date).NotEmpty().WithMessage("Sale date is required.");
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Customer ID is required.");
        RuleFor(x => x.CustomerName).NotEmpty().WithMessage("Customer name is required.");
        RuleFor(x => x.BranchId).NotEmpty().WithMessage("Branch ID is required.");
        RuleFor(x => x.BranchName).NotEmpty().WithMessage("Branch name is required.");
        RuleFor(x => x.Items).NotEmpty().WithMessage("Sale must contain at least one item.");
    }
}