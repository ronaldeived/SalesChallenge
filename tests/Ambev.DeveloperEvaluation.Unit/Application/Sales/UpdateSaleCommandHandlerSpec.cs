using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleCommandHandlerSpec
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly Mock<ISaleRepository> _saleRepository;
    private readonly UpdateSaleCommandHandler _handler;

    public UpdateSaleCommandHandlerSpec()
    {
        _saleRepository = _fixture.Freeze<Mock<ISaleRepository>>();
        _handler = _fixture.Create<UpdateSaleCommandHandler>();
    }

    /// <summary>
    /// Tests that an existing sale is updated successfully with new header and items.
    /// </summary>
    [Fact]
    public async Task Should_Update_Sale_Successfully()
    {
        // Arrange
        var existingSale = Sale.Create("S-3001", DateTime.UtcNow.AddDays(-1), Guid.NewGuid(), "Old Customer", Guid.NewGuid(), "Old Branch");
        existingSale.AddItem(new SaleItem(Guid.NewGuid(), "Old Product", 1, 10m));

        _saleRepository.Setup(r => r.GetByIdAsNoTrackingAsync(existingSale.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSale);

        var command = new UpdateSaleCommand(
            existingSale.Id,
            "S-3001-Updated",
            DateTime.UtcNow,
            Guid.NewGuid(),
            "New Customer",
            Guid.NewGuid(),
            "New Branch",
            new List<UpdateSaleItemInput>
            {
                new(Guid.NewGuid(), Guid.NewGuid(), "New Product", 2, 15m)
            }
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Number.Should().Be("S-3001-Updated");
        result.CustomerName.Should().Be("New Customer");
        result.Items.Should().HaveCount(1);
        result.Items.First().ProductName.Should().Be("New Product");
        result.Total.Should().Be(30m);

        _saleRepository.Verify(r => r.UpdateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that updating a non-existing sale returns null.
    /// </summary>
    [Fact]
    public async Task Should_Return_Null_When_Sale_Not_Found()
    {
        // Arrange
        _saleRepository.Setup(r => r.GetByIdAsNoTrackingAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale?)null);

        var command = new UpdateSaleCommand(
            Guid.NewGuid(),
            "S-9999",
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Ghost Customer",
            Guid.NewGuid(),
            "Ghost Branch",
            new List<UpdateSaleItemInput>
            {
                new(Guid.NewGuid(), Guid.NewGuid(), "Nonexistent Product", 1, 10m)
            }
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _saleRepository.Verify(r => r.UpdateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Tests that business rules (discounts) are applied correctly when updating items.
    /// </summary>
    [Fact]
    public async Task Should_Apply_Discount_When_Updating_Sale_Items()
    {
        // Arrange
        var existingSale = Sale.Create("S-3002", DateTime.UtcNow.AddDays(-2), Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        existingSale.AddItem(new SaleItem(Guid.NewGuid(), "Old Product", 2, 10m));

        _saleRepository.Setup(r => r.GetByIdAsNoTrackingAsync(existingSale.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSale);

        var command = new UpdateSaleCommand(
            existingSale.Id,
            "S-3002-Updated",
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Customer",
            Guid.NewGuid(),
            "Branch",
            new List<UpdateSaleItemInput>
            {
                new(Guid.NewGuid(), Guid.NewGuid(), "Product A", 5, 10m)
            }
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var updatedItem = result!.Items.First();
        updatedItem.DiscountPercent.Should().Be(10m);
        updatedItem.Total.Should().Be(45m);
    }
}