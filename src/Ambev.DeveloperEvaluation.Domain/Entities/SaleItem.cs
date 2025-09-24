namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid SaleId { get; private set; }

    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;

    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// Discount percent as an integer value (0, 10, 20).
    /// </summary>
    public decimal DiscountPercent { get; private set; }

    /// <summary>
    /// Total after discount.
    /// </summary>
    public decimal Total => Math.Round(Quantity * UnitPrice * (1 - (DiscountPercent / 100)), 2);

    private SaleItem() { }

    public SaleItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        ProductName = productName;

        SetQuantity(quantity);
        SetUnitPrice(unitPrice);
        ApplyDiscount();
    }

    public void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        if (quantity > 20)
            throw new InvalidOperationException("Cannot sell more than 20 units of the same product.");

        Quantity = quantity;
        ApplyDiscount();
    }

    public void SetUnitPrice(decimal unitPrice)
    {
        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero.");

        UnitPrice = unitPrice;
    }

    private void ApplyDiscount()
    {
        if (Quantity >= 10)
        {
            DiscountPercent = 20;
        }
        else if (Quantity >= 4)
        {
            DiscountPercent = 10;
        }
        else
        {
            DiscountPercent = 0;
        }
    }
}
