using FluentAssertions;
using InnoShop.ProductManagement.Application.Products.Queries.ListProducts;
using InnoShop.ProductManagement.Application.SubcutaneousTests.Common;
using InnoShop.ProductManagement.Domain.ProductAggregate;
using InnoShop.ProductManagement.Infrastructure.Persistence;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace InnoShop.ProductManagement.Application.SubcutaneousTests.Products.Queries;

[Collection(MediatorFactoryCollection.CollectionName)]
public class ListProductsQueryTests(MediatorFactory mediatorFactory)
{
    [Fact]
    public async Task ListProducts_WithSearchTerm_ShouldFilterByTitle()
    {
        // Arrange
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductManagementDbContext>();

        var sellerId = mediatorFactory.DefaultUserId;
        var sellerInfo = new SellerSnapshot("Test Seller", "", 0.0, 0);
        mediatorFactory.SetupUserGateway(sellerId, sellerInfo);

        var price = Price.Create(100m).Value;

        var product1 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("iPhone 11").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);

        var product2 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("iPhone 12").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);

        var product3 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Samsung Galaxy").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);

        dbContext.Products.AddRange(product1, product2, product3);
        await dbContext.SaveChangesAsync();

        // Act
        var query = new ListProductsQuery(
            1,
            10,
            "iPhone");

        var result = await mediator.Send(query);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().HaveCount(2);
        result.Value.Items.Should().OnlyContain(p => p.Title.Contains("iPhone", StringComparison.OrdinalIgnoreCase));
        result.Value.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task ListProducts_WithPriceFilter_ShouldFilterCorrectly()
    {
        // Arrange
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductManagementDbContext>();

        var sellerId = mediatorFactory.DefaultUserId;
        var sellerInfo = new SellerSnapshot("Test Seller", "", 0.0, 0);
        mediatorFactory.SetupUserGateway(sellerId, sellerInfo);

        var product1 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Product 1").Value,
            Description.Create("Description").Value,
            Price.Create(50m).Value,
            sellerId,
            sellerInfo);

        var product2 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Product 2").Value,
            Description.Create("Description").Value,
            Price.Create(100m).Value,
            sellerId,
            sellerInfo);

        var product3 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Product 3").Value,
            Description.Create("Description").Value,
            Price.Create(150m).Value,
            sellerId,
            sellerInfo);

        dbContext.Products.AddRange(product1, product2, product3);
        await dbContext.SaveChangesAsync();

        // Act
        var query = new ListProductsQuery(
            1,
            10,
            MinPrice: 75m,
            MaxPrice: 125m);

        var result = await mediator.Send(query);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items[0].Price.Should().Be(100m);
        result.Value.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task ListProducts_WithPagination_ShouldCalculateTotalPagesCorrectly()
    {
        // Arrange
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductManagementDbContext>();

        var sellerId = mediatorFactory.DefaultUserId;
        var sellerInfo = new SellerSnapshot("Test Seller", "", 0.0, 0);
        mediatorFactory.SetupUserGateway(sellerId, sellerInfo);

        var price = Price.Create(100m).Value;

        for (var i = 1; i <= 15; i++)
        {
            var product = Product.CreateProduct(
                Guid.NewGuid(),
                Title.Create($"Product {i}").Value,
                Description.Create("Description").Value,
                price,
                sellerId,
                sellerInfo);
            dbContext.Products.Add(product);
        }

        await dbContext.SaveChangesAsync();

        // Act
        var query = new ListProductsQuery(
            2,
            5);

        var result = await mediator.Send(query);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().HaveCount(5);
        result.Value.TotalCount.Should().Be(15);
        result.Value.TotalPages.Should().Be(3);
        result.Value.Page.Should().Be(2);
        result.Value.PageSize.Should().Be(5);
    }

    [Fact]
    public async Task ListProducts_ByDefault_ShouldReturnOnlyActiveProducts()
    {
        // Arrange
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductManagementDbContext>();

        var sellerId = mediatorFactory.DefaultUserId;
        var sellerInfo = new SellerSnapshot("Test Seller", "", 0.0, 0);
        mediatorFactory.SetupUserGateway(sellerId, sellerInfo);

        var price = Price.Create(100m).Value;

        var product1 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Product 1").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);

        var product2 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Product 2").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);
        product2.Hide();

        var product3 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Product 3").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);

        dbContext.Products.AddRange(product1, product2, product3);
        await dbContext.SaveChangesAsync();

        // Act
        var query = new ListProductsQuery();

        var result = await mediator.Send(query);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().HaveCount(2);
        result.Value.Items.Should().OnlyContain(p => p.IsActive);
        result.Value.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task ListProducts_WithIsAvailableTrue_ShouldReturnOnlyActiveProducts()
    {
        // Arrange
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductManagementDbContext>();

        var sellerId = mediatorFactory.DefaultUserId;
        var sellerInfo = new SellerSnapshot("Test Seller", "", 0.0, 0);
        mediatorFactory.SetupUserGateway(sellerId, sellerInfo);

        var price = Price.Create(100m).Value;

        var product1 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Product 1").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);

        var product2 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Product 2").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);
        product2.Hide();

        dbContext.Products.AddRange(product1, product2);
        await dbContext.SaveChangesAsync();

        // Act
        var query = new ListProductsQuery(IsAvailable: true);

        var result = await mediator.Send(query);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items[0].IsActive.Should().BeTrue();
        result.Value.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task ListProducts_WithIsAvailableFalse_ShouldReturnOnlyInactiveProducts()
    {
        // Arrange
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductManagementDbContext>();

        var sellerId = mediatorFactory.DefaultUserId;
        var sellerInfo = new SellerSnapshot("Test Seller", "", 0.0, 0);
        mediatorFactory.SetupUserGateway(sellerId, sellerInfo);

        var price = Price.Create(100m).Value;

        var product1 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Product 1").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);

        var product2 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Product 2").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);
        product2.Hide();

        dbContext.Products.AddRange(product1, product2);
        await dbContext.SaveChangesAsync();

        // Act
        var query = new ListProductsQuery(IsAvailable: false);

        var result = await mediator.Send(query);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items[0].IsActive.Should().BeFalse();
        result.Value.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task ListProducts_WithFiltersAndPagination_ShouldCalculateCorrectly()
    {
        // Arrange
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductManagementDbContext>();

        var sellerId = mediatorFactory.DefaultUserId;
        var sellerInfo = new SellerSnapshot("Test Seller", "", 0.0, 0);
        mediatorFactory.SetupUserGateway(sellerId, sellerInfo);

        // Create 10 products with price 100, 5 with price 200
        for (var i = 1; i <= 10; i++)
        {
            var product = Product.CreateProduct(
                Guid.NewGuid(),
                Title.Create($"Product {i}").Value,
                Description.Create("Description").Value,
                Price.Create(100m).Value,
                sellerId,
                sellerInfo);
            dbContext.Products.Add(product);
        }

        for (var i = 11; i <= 15; i++)
        {
            var product = Product.CreateProduct(
                Guid.NewGuid(),
                Title.Create($"Product {i}").Value,
                Description.Create("Description").Value,
                Price.Create(200m).Value,
                sellerId,
                sellerInfo);
            dbContext.Products.Add(product);
        }

        await dbContext.SaveChangesAsync();

        // Act - Filter by price >= 150, page 1, page size 5
        var query = new ListProductsQuery(
            1,
            5,
            MinPrice: 150m);

        var result = await mediator.Send(query);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().HaveCount(5);
        result.Value.Items.Should().OnlyContain(p => p.Price >= 150m);
        result.Value.TotalCount.Should().Be(5); // Only products with price >= 150
        result.Value.TotalPages.Should().Be(1); // 5 items / 5 page size = 1 page
    }
}