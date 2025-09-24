using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEventEntity
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Number { get; private set; } = string.Empty;
    public DateTime Date { get; private set; }

    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;

    public Guid BranchId { get; private set; }
    public string BranchName { get; private set; } = string.Empty;

    public bool IsCancelled { get; private set; }

    public List<SaleItem> Items { get; private set; } = new();

    public decimal Total => Math.Round(Items.Sum(i => i.Total), 2);

    private Sale(string number, DateTime date, Guid customerId, string customerName, Guid branchId, string branchName)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Sale number is required.");

        Number = number;
        Date = date;
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;
        IsCancelled = false;
    }
    
    public static Sale Create(string number, DateTime date, Guid customerId, string customerName, 
        Guid branchId, string branchName)
    {
        var sale = new Sale(number, date, customerId, customerName, branchId, branchName);
        
        sale.AddEvent(new SaleCreatedEvent(sale.Id, DateTime.UtcNow));
        
        return sale;
    }

    public void UpdateHeader(string number, DateTime date, Guid customerId, string customerName, Guid branchId, string branchName)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot update a cancelled sale.");

        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Sale number is required.");

        Number = number;
        Date = date;
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;
        
        AddEvent(new SaleModifiedEvent(Id, DateTime.UtcNow));
    }

    public void ReplaceItems(IEnumerable<SaleItem> items)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot modify items on a cancelled sale.");

        Items = items.ToList();
    }

    public void AddItem(SaleItem item)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot add items to a cancelled sale.");

        Items.Add(item);
    }

    public void Cancel()
    {
        if (IsCancelled)
            throw new InvalidOperationException("Sale is already cancelled.");

        IsCancelled = true;
        
        AddEvent(new SaleCancelledEvent(Id, DateTime.UtcNow));
    }
}
