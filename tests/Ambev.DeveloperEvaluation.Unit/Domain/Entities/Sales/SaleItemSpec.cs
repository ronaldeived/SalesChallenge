using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.Sales;

public class SaleItemSpec
{
    /// <summary>
    /// Tests that a SaleItem with quantity less than 4 has no discount.
    /// </summary>
    [Fact]
    public void Should_Not_Apply_Discount_When_Quantity_Less_Than_4()
    {
        var item = new SaleItem(Guid.NewGuid(), "Product A", 3, 10m);

        item.DiscountPercent.Should().Be(0);
        item.Total.Should().Be(30m);
    }

    /// <summary>
    /// Tests that a SaleItem with quantity between 4 and 9 has 10% discount.
    /// </summary>
    [Fact]
    public void Should_Apply_10_Percent_Discount_When_Quantity_Between_4_And_9()
    {
        var item = new SaleItem(Guid.NewGuid(), "Product B", 5, 20m);

        item.DiscountPercent.Should().Be(10);
        item.Total.Should().Be(90m); // 5 * 20 * 0.9
    }

    /// <summary>
    /// Tests that a SaleItem with quantity between 10 and 20 has 20% discount.
    /// </summary>
    [Fact]
    public void Should_Apply_20_Percent_Discount_When_Quantity_Between_10_And_20()
    {
        var item = new SaleItem(Guid.NewGuid(), "Product C", 15, 10m);

        item.DiscountPercent.Should().Be(20);
        item.Total.Should().Be(120m); // 15 * 10 * 0.8
    }

    /// <summary>
    /// Tests that creating a SaleItem with more than 20 units throws an exception.
    /// </summary>
    [Fact]
    public void Should_Throw_Exception_When_Quantity_Greater_Than_20()
    {
        Action act = () => new SaleItem(Guid.NewGuid(), "Product D", 21, 5m);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot sell more than 20 units of the same product.");
    }

    /// <summary>
    /// Tests that creating a SaleItem with zero or negative quantity throws an exception.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Should_Throw_Exception_When_Quantity_Is_Zero_Or_Negative(int invalidQty)
    {
        Action act = () => new SaleItem(Guid.NewGuid(), "Product E", invalidQty, 5m);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Quantity must be greater than zero.");
    }
}
