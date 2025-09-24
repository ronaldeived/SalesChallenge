using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Unit tests for the CreateSaleCommandHandler.
/// </summary>
public class CreateSaleCommandHandlerSpec
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly Mock<ISaleRepository> _saleRepository;
    private readonly CreateSaleCommandHandler _handler;

    public CreateSaleCommandHandlerSpec()
    {
        _saleRepository = _fixture.Freeze<Mock<ISaleRepository>>();
        _handler = _fixture.Create<CreateSaleCommandHandler>();
    }

    /// <summary>
    /// Tests that a valid sale creation request with two items is handled successfully.
    /// </summary>
    [Fact]
    public async Task Should_Create_Sale_Successfully()
    {
        // Arrange
        var command = _fixture.Build<CreateSaleCommand>()
            .With(c => c.Items, new List<CreateSaleItemInput>
            {
                new(Guid.NewGuid(), "Product 1", 2, 10m),
                new(Guid.NewGuid(), "Product 2", 1, 20m)
            })
            .Create();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Total.Should().Be(40m);
        _saleRepository.Verify(r => r.AddAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that a 10% discount is applied when the same product has a quantity between 4 and 9.
    /// </summary>
    [Fact]
    public async Task Should_Apply_10_Percent_Discount_When_Same_Product_Quantity_Between_4_And_9()
    {
        var items = new List<CreateSaleItemInput>
        {
            new(Guid.NewGuid(), "Product A", 5, 10m)
        };

        var command = _fixture.Build<CreateSaleCommand>()
            .With(c => c.Items, items)
            .With(c => c.Number, "S-2001")
            .Create();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().DiscountPercent.Should().Be(10);
        result.Total.Should().Be(45m);
    }

    /// <summary>
    /// Tests that creating a sale with more than 20 units of the same product throws a domain exception.
    /// </summary>
    [Fact]
    public async Task Should_Throw_Exception_When_Same_Product_Quantity_Greater_Than_20()
    {
        var items = new List<CreateSaleItemInput>
        {
            new(Guid.NewGuid(), "Product A", 21, 5m)
        };

        var command = _fixture.Build<CreateSaleCommand>()
            .With(c => c.Items, items)
            .With(c => c.Number, "S-2002")
            .Create();

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot sell more than 20 units of the same product.");
    }
}