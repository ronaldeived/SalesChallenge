using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.Sales;

public class SaleSpec
{
    /// <summary>
    /// Tests that creating a Sale with valid data initializes correctly.
    /// </summary>
    [Fact]
    public void Should_Create_Sale_With_Valid_Data()
    {
        var sale = Sale.Create("S-1001", DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");

        sale.Should().NotBeNull();
        sale.Number.Should().Be("S-1001");
        sale.IsCancelled.Should().BeFalse();
        sale.Items.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that creating a Sale with an empty number throws an exception.
    /// </summary>
    [Fact]
    public void Should_Throw_Exception_When_Creating_Sale_With_Empty_Number()
    {
        Action act = () => Sale.Create("", DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Sale number is required.");
    }

    /// <summary>
    /// Tests that a Sale header can be updated with valid data.
    /// </summary>
    [Fact]
    public void Should_Update_Sale_Header_Successfully()
    {
        var sale = Sale.Create("S-1002", DateTime.UtcNow, Guid.NewGuid(), "Customer A", Guid.NewGuid(), "Branch A");

        sale.UpdateHeader("S-1002-Updated", DateTime.UtcNow.AddDays(1), Guid.NewGuid(), "Customer B", Guid.NewGuid(), "Branch B");

        sale.Number.Should().Be("S-1002-Updated");
        sale.CustomerName.Should().Be("Customer B");
        sale.BranchName.Should().Be("Branch B");
    }

    /// <summary>
    /// Tests that updating a cancelled Sale throws an exception.
    /// </summary>
    [Fact]
    public void Should_Throw_Exception_When_Updating_A_Cancelled_Sale()
    {
        var sale = Sale.Create("S-1003", DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        sale.Cancel();

        Action act = () => sale.UpdateHeader("S-1003-Updated", DateTime.UtcNow, Guid.NewGuid(), "Customer B", Guid.NewGuid(), "Branch B");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot update a cancelled sale.");
    }

    /// <summary>
    /// Tests that items can be added to a Sale.
    /// </summary>
    [Fact]
    public void Should_Add_Items_To_Sale()
    {
        var sale = Sale.Create("S-1004", DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        var item = new SaleItem(Guid.NewGuid(), "Product A", 2, 10m);

        sale.AddItem(item);

        sale.Items.Should().ContainSingle();
        sale.Total.Should().Be(20m);
    }

    /// <summary>
    /// Tests that items can be replaced in a Sale.
    /// </summary>
    [Fact]
    public void Should_Replace_Items_In_Sale()
    {
        var sale = Sale.Create("S-1005", DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        sale.AddItem(new SaleItem(Guid.NewGuid(), "Product A", 2, 10m));

        var newItems = new List<SaleItem>
        {
            new(Guid.NewGuid(), "Product B", 5, 5m)
        };

        sale.ReplaceItems(newItems);

        sale.Items.Should().ContainSingle();
        sale.Items.First().ProductName.Should().Be("Product B");
        sale.Items.First().DiscountPercent.Should().Be(10);
        sale.Total.Should().Be(22.5m);
    }

    /// <summary>
    /// Tests that cancelling a Sale sets IsCancelled to true.
    /// </summary>
    [Fact]
    public void Should_Cancel_Sale()
    {
        var sale = Sale.Create("S-1006", DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");

        sale.Cancel();

        sale.IsCancelled.Should().BeTrue();
    }

    /// <summary>
    /// Tests that cancelling an already cancelled Sale throws an exception.
    /// </summary>
    [Fact]
    public void Should_Throw_Exception_When_Cancelling_Already_Cancelled_Sale()
    {
        var sale = Sale.Create("S-1007", DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        sale.Cancel();

        Action act = () => sale.Cancel();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Sale is already cancelled.");
    }

    /// <summary>
    /// Tests that adding items to a cancelled Sale throws an exception.
    /// </summary>
    [Fact]
    public void Should_Throw_Exception_When_Adding_Items_To_Cancelled_Sale()
    {
        var sale = Sale.Create("S-1008", DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        sale.Cancel();

        Action act = () => sale.AddItem(new SaleItem(Guid.NewGuid(), "Product A", 1, 10m));

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot add items to a cancelled sale.");
    }

    /// <summary>
    /// Tests that replacing items in a cancelled Sale throws an exception.
    /// </summary>
    [Fact]
    public void Should_Throw_Exception_When_Replacing_Items_In_Cancelled_Sale()
    {
        var sale = Sale.Create("S-1009", DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        sale.Cancel();

        Action act = () => sale.ReplaceItems(new List<SaleItem> { new(Guid.NewGuid(), "Product B", 2, 10m) });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot modify items on a cancelled sale.");
    }
}