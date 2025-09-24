using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class DeleteSaleCommandHandlerSpec
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly Mock<ISaleRepository> _saleRepository;
    private readonly DeleteSaleCommandHandler _handler;

    public DeleteSaleCommandHandlerSpec()
    {
        _saleRepository = _fixture.Freeze<Mock<ISaleRepository>>();
        _handler = _fixture.Create<DeleteSaleCommandHandler>();
    }

    /// <summary>
    /// Tests that deleting an existing sale returns true when it no longer exists in the repository.
    /// </summary>
    [Fact]
    public async Task Should_Delete_Sale_Successfully()
    {
        // Arrange
        var sale = Sale.Create("S-3001", DateTime.UtcNow, Guid.NewGuid(), "Customer C", Guid.NewGuid(), "Branch C");

        _saleRepository.Setup(r => r.DeleteAsync(sale.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _saleRepository.Setup(r => r.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale?)null);

        var command = new DeleteSaleCommand(sale.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _saleRepository.Verify(r => r.DeleteAsync(sale.Id, It.IsAny<CancellationToken>()), Times.Once);
        _saleRepository.Verify(r => r.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that deleting a sale that still exists afterwards returns false.
    /// </summary>
    [Fact]
    public async Task Should_Return_False_When_Sale_Still_Exists()
    {
        // Arrange
        var sale = Sale.Create("S-3002", DateTime.UtcNow, Guid.NewGuid(), "Customer D", Guid.NewGuid(), "Branch D");

        _saleRepository.Setup(r => r.DeleteAsync(sale.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _saleRepository.Setup(r => r.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale); // ainda existe

        var command = new DeleteSaleCommand(sale.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _saleRepository.Verify(r => r.DeleteAsync(sale.Id, It.IsAny<CancellationToken>()), Times.Once);
        _saleRepository.Verify(r => r.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}