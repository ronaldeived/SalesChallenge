using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public class SaleCancelledEventHandler : INotificationHandler<SaleCancelledEvent>
{
    public Task Handle(SaleCancelledEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[EVENT] Sale cancelled: {notification.SaleId} at {notification.OccurredOn}");
        return Task.CompletedTask;
    }
}
