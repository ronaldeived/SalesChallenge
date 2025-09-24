using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class GetSaleByIdQueryHandlerSpec
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly Mock<ISaleRepository> _saleRepository;
    private readonly GetSaleByIdQueryHandler _handler;

    public GetSaleByIdQueryHandlerSpec()
    {
        _saleRepository = _fixture.Freeze<Mock<ISaleRepository>>();
        _handler = _fixture.Create<GetSaleByIdQueryHandler>();
    }

    /// <summary>
    /// Tests that querying an existing sale by ID returns a valid SaleDto.
    /// </summary>
    [Fact]
    public async Task Should_Return_SaleDto_When_Sale_Exists()
    {
        // Arrange
        var sale = Sale.Create("S-4001", DateTime.UtcNow, Guid.NewGuid(), "Customer E", Guid.NewGuid(), "Branch E");
        sale.AddItem(new SaleItem(Guid.NewGuid(), "Product X", 2, 15m));

        _saleRepository.Setup(r => r.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var query = new GetSaleByIdQuery(sale.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(sale.Id);
        result.CustomerName.Should().Be("Customer E");
        result.Items.Should().HaveCount(1);
        result.Total.Should().Be(30m);
        _saleRepository.Verify(r => r.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that querying a non-existing sale by ID returns null.
    /// </summary>
    [Fact]
    public async Task Should_Return_Null_When_Sale_Not_Found()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        _saleRepository.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale?)null);

        var query = new GetSaleByIdQuery(saleId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _saleRepository.Verify(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>()), Times.Once);
    }
}