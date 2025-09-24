using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CancelSaleCommandHandlerSpec
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly Mock<ISaleRepository> _saleRepository;
    private readonly CancelSaleCommandHandler _handler;

    public CancelSaleCommandHandlerSpec()
    {
        _saleRepository = _fixture.Freeze<Mock<ISaleRepository>>();
        _handler = _fixture.Create<CancelSaleCommandHandler>();
    }

    /// <summary>
    /// Tests that cancelling an existing sale sets IsCancelled to true and updates the repository.
    /// </summary>
    [Fact]
    public async Task Should_Cancel_Sale_Successfully()
    {
        // Arrange
        var sale = Sale.Create("S-1001", DateTime.UtcNow, Guid.NewGuid(), "Customer A", Guid.NewGuid(), "Branch A");
        _saleRepository.Setup(r => r.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new CancelSaleCommand(sale.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.IsCancelled.Should().BeTrue();
        _saleRepository.Verify(r => r.UpdateAsync(sale, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that cancelling a sale that does not exist returns null.
    /// </summary>
    [Fact]
    public async Task Should_Return_Null_When_Sale_Not_Found()
    {
        // Arrange
        var command = new CancelSaleCommand(Guid.NewGuid());
        _saleRepository.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _saleRepository.Verify(r => r.UpdateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Tests that cancelling an already canceled sale throws an exception.
    /// </summary>
    [Fact]
    public async Task Should_Throw_Exception_When_Sale_Already_Cancelled()
    {
        // Arrange
        var sale = Sale.Create("S-2001", DateTime.UtcNow, Guid.NewGuid(), "Customer B", Guid.NewGuid(), "Branch B");
        sale.Cancel();

        _saleRepository.Setup(r => r.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new CancelSaleCommand(sale.Id);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Unable to cancel the sale.*");

        _saleRepository.Verify(r => r.UpdateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}