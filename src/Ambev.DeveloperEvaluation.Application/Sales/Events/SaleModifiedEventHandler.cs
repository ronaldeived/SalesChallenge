using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public class SaleModifiedEventHandler : INotificationHandler<SaleModifiedEvent>
{
    public Task Handle(SaleModifiedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[EVENT] Sale modified: {notification.SaleId} at {notification.OccurredOn}");
        return Task.CompletedTask;
    }
}