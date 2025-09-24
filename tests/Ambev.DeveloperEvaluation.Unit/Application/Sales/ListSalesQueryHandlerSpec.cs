using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class ListSalesQueryHandlerSpec
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly Mock<ISaleRepository> _saleRepository;
    private readonly ListSalesQueryHandler _handler;

    public ListSalesQueryHandlerSpec()
    {
        _saleRepository = _fixture.Freeze<Mock<ISaleRepository>>();
        _handler = _fixture.Create<ListSalesQueryHandler>();
    }

    /// <summary>
    /// Tests that when sales exist, the handler returns a list of SaleDto with correct mapping.
    /// </summary>
    [Fact]
    public async Task Should_Return_Sales_When_They_Exist()
    {
        // Arrange
        var sale1 = Sale.Create("S-5001", DateTime.UtcNow, Guid.NewGuid(), "Customer A", Guid.NewGuid(), "Branch A");
        sale1.AddItem(new SaleItem(Guid.NewGuid(), "Product 1", 2, 10m));

        var sale2 = Sale.Create("S-5002", DateTime.UtcNow, Guid.NewGuid(), "Customer B", Guid.NewGuid(), "Branch B");
        sale2.AddItem(new SaleItem(Guid.NewGuid(), "Product 2", 1, 20m));

        var sales = new List<Sale> { sale1, sale2 };

        _saleRepository.Setup(r => r.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sales);

        var query = new ListSalesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var salesList = result.ToList();
        salesList.Should().NotBeNull();
        salesList.Should().HaveCount(2);
        salesList.First().CustomerName.Should().Be("Customer A");
        salesList.Last().CustomerName.Should().Be("Customer B");
        salesList.Sum(s => s.Total).Should().Be(40m);
        _saleRepository.Verify(r => r.ListAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that when no sales exist, the handler returns an empty list.
    /// </summary>
    [Fact]
    public async Task Should_Return_Empty_List_When_No_Sales_Exist()
    {
        // Arrange
        _saleRepository.Setup(r => r.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Sale>());

        var query = new ListSalesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var salesList = result.ToList();
        salesList.Should().NotBeNull();
        salesList.Should().BeEmpty();
        _saleRepository.Verify(r => r.ListAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}