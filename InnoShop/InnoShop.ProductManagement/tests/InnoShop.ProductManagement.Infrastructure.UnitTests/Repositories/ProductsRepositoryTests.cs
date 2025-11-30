using FluentAssertions;
using InnoShop.ProductManagement.Domain.ProductAggregate;
using InnoShop.ProductManagement.Infrastructure.Persistence;
using InnoShop.ProductManagement.Infrastructure.Persistence.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace InnoShop.ProductManagement.Infrastructure.UnitTests.Repositories;

public class ProductsRepositoryTests : IDisposable
{
    private readonly ProductManagementDbContext _dbContext;
    private readonly ProductsRepository _repository;

    public ProductsRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ProductManagementDbContext>()
            .UseSqlite("DataSource=:memory:")
            .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
            .Options;

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var publisher = Substitute.For<IPublisher>();
        var logger = Substitute.For<ILogger<ProductManagementDbContext>>();

        _dbContext = new ProductManagementDbContext(options, httpContextAccessor, publisher, logger);
        _dbContext.Database.EnsureCreated();

        _repository = new ProductsRepository(_dbContext);
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }

    [Fact]
    public async Task GetPagedAsync_WithSearchTerm_ShouldFilterByTitle()
    {
        // Adding urls for each snapshot
        // Arrange
        var sellerId = Guid.NewGuid();
        var sellerInfo = new SellerSnapshot("Test Seller", "https://example.com/avatar.jpg", 0.0, 0);
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
            Title.Create("Samsung Galaxy").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);

        _dbContext.Products.AddRange(product1, product2);
        await _dbContext.SaveChangesAsync();

        // Act
        var (products, totalCount) = await _repository.GetPagedAsync(
            1,
            10,
            "iPhone");

        // Assert
        products.Should().HaveCount(1);
        products[0].Title.Should().Be("iPhone 11");
        totalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetPagedAsync_WithSearchTerm_ShouldFilterByDescription()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var sellerInfo = new SellerSnapshot("Test Seller", "https://example.com/avatar.jpg", 0.0, 0);
        var price = Price.Create(100m).Value;

        var product1 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Product 1").Value,
            Description.Create("iPhone description").Value,
            price,
            sellerId,
            sellerInfo);

        var product2 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Product 2").Value,
            Description.Create("Samsung description").Value,
            price,
            sellerId,
            sellerInfo);

        _dbContext.Products.AddRange(product1, product2);
        await _dbContext.SaveChangesAsync();

        // Act
        var (products, totalCount) = await _repository.GetPagedAsync(
            1,
            10,
            "iPhone");

        // Assert
        products.Should().HaveCount(1);
        products[0].Description.Should().Be("iPhone description");
        totalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetPagedAsync_WithSearchTerm_ShouldFilterByTitleOrDescription()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var sellerInfo = new SellerSnapshot("Test Seller", "https://example.com/avatar.jpg", 0.0, 0);
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
            Title.Create("Product 2").Value,
            Description.Create("iPhone description").Value,
            price,
            sellerId,
            sellerInfo);

        var product3 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Samsung").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);

        _dbContext.Products.AddRange(product1, product2, product3);
        await _dbContext.SaveChangesAsync();

        // Act
        var (products, totalCount) = await _repository.GetPagedAsync(
            1,
            10,
            "iPhone");

        // Assert
        products.Should().HaveCount(2);
        totalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetPagedAsync_WithMinPrice_ShouldFilterCorrectly()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var sellerInfo = new SellerSnapshot("Test Seller", "https://example.com/avatar.jpg", 0.0, 0);

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

        _dbContext.Products.AddRange(product1, product2, product3);
        await _dbContext.SaveChangesAsync();

        // Act
        var (products, totalCount) = await _repository.GetPagedAsync(
            1,
            10,
            minPrice: 100m);

        // Assert
        products.Should().HaveCount(2);
        products.All(p => p.Price.Value >= 100m).Should().BeTrue();
        totalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetPagedAsync_WithMaxPrice_ShouldFilterCorrectly()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var sellerInfo = new SellerSnapshot("Test Seller", "https://example.com/avatar.jpg", 0.0, 0);

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

        _dbContext.Products.AddRange(product1, product2, product3);
        await _dbContext.SaveChangesAsync();

        // Act
        var (products, totalCount) = await _repository.GetPagedAsync(
            1,
            10,
            maxPrice: 100m);

        // Assert
        products.Should().HaveCount(2);
        products.All(p => p.Price.Value <= 100m).Should().BeTrue();
        totalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetPagedAsync_WithPriceRange_ShouldFilterCorrectly()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var sellerInfo = new SellerSnapshot("Test Seller", "https://example.com/avatar.jpg", 0.0, 0);

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

        _dbContext.Products.AddRange(product1, product2, product3);
        await _dbContext.SaveChangesAsync();

        // Act
        var (products, totalCount) = await _repository.GetPagedAsync(
            1,
            10,
            minPrice: 75m,
            maxPrice: 125m);

        // Assert
        products.Should().HaveCount(1);
        products[0].Price.Value.Should().Be(100m);
        totalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetPagedAsync_WithIsAvailableTrue_ShouldReturnOnlyActiveProducts()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var sellerInfo = new SellerSnapshot("Test Seller", "https://example.com/avatar.jpg", 0.0, 0);
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

        _dbContext.Products.AddRange(product1, product2);
        await _dbContext.SaveChangesAsync();

        // Act
        var (products, totalCount) = await _repository.GetPagedAsync(
            1,
            10,
            isAvailable: true);

        // Assert
        products.Should().HaveCount(1);
        products[0].IsActive.Should().BeTrue();
        totalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetPagedAsync_WithIsAvailableFalse_ShouldReturnOnlyInactiveProducts()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var sellerInfo = new SellerSnapshot("Test Seller", "https://example.com/avatar.jpg", 0.0, 0);
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

        _dbContext.Products.AddRange(product1, product2);
        await _dbContext.SaveChangesAsync();

        // Act
        var (products, totalCount) = await _repository.GetPagedAsync(
            1,
            10,
            isAvailable: false);

        // Assert
        products.Should().HaveCount(1);
        products[0].IsActive.Should().BeFalse();
        totalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetPagedAsync_WithoutIsAvailable_ShouldReturnOnlyActiveProductsByDefault()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var sellerInfo = new SellerSnapshot("Test Seller", "https://example.com/avatar.jpg", 0.0, 0);
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

        _dbContext.Products.AddRange(product1, product2);
        await _dbContext.SaveChangesAsync();

        // Act
        var (products, totalCount) = await _repository.GetPagedAsync(
            1,
            10);

        // Assert
        products.Should().HaveCount(1);
        products[0].IsActive.Should().BeTrue();
        totalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetPagedAsync_WithSortByTitleAscending_ShouldSortCorrectly()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var sellerInfo = new SellerSnapshot("Test Seller", "https://example.com/avatar.jpg", 0.0, 0);
        var price = Price.Create(100m).Value;

        var product1 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Zebra").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);

        var product2 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Apple").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);

        var product3 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Banana").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);

        _dbContext.Products.AddRange(product1, product2, product3);
        await _dbContext.SaveChangesAsync();

        // Act
        var (products, totalCount) = await _repository.GetPagedAsync(
            1,
            10,
            sortBy: "title",
            sortOrder: "asc");

        // Assert
        products.Should().HaveCount(3);
        products[0].Title.Should().Be("Apple");
        products[1].Title.Should().Be("Banana");
        products[2].Title.Should().Be("Zebra");
    }

    [Fact]
    public async Task GetPagedAsync_WithSortByTitleDescending_ShouldSortCorrectly()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var sellerInfo = new SellerSnapshot("Test Seller", "https://example.com/avatar.jpg", 0.0, 0);
        var price = Price.Create(100m).Value;

        var product1 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Apple").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);

        var product2 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Banana").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);

        var product3 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Zebra").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);

        _dbContext.Products.AddRange(product1, product2, product3);
        await _dbContext.SaveChangesAsync();

        // Act
        var (products, totalCount) = await _repository.GetPagedAsync(
            1,
            10,
            sortBy: "title",
            sortOrder: "desc");

        // Assert
        products.Should().HaveCount(3);
        products[0].Title.Should().Be("Zebra");
        products[1].Title.Should().Be("Banana");
        products[2].Title.Should().Be("Apple");
    }

    [Fact]
    public async Task GetPagedAsync_WithSortByPriceAscending_ShouldSortCorrectly()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var sellerInfo = new SellerSnapshot("Test Seller", "https://example.com/avatar.jpg", 0.0, 0);

        var product1 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Product 1").Value,
            Description.Create("Description").Value,
            Price.Create(300m).Value,
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
            Price.Create(200m).Value,
            sellerId,
            sellerInfo);

        _dbContext.Products.AddRange(product1, product2, product3);
        await _dbContext.SaveChangesAsync();

        // Act
        var (products, totalCount) = await _repository.GetPagedAsync(
            1,
            10,
            sortBy: "price",
            sortOrder: "asc");

        // Assert
        products.Should().HaveCount(3);
        products[0].Price.Value.Should().Be(100m);
        products[1].Price.Value.Should().Be(200m);
        products[2].Price.Value.Should().Be(300m);
    }

    [Fact]
    public async Task GetPagedAsync_WithSortByPriceDescending_ShouldSortCorrectly()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var sellerInfo = new SellerSnapshot("Test Seller", "https://example.com/avatar.jpg", 0.0, 0);

        var product1 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Product 1").Value,
            Description.Create("Description").Value,
            Price.Create(100m).Value,
            sellerId,
            sellerInfo);

        var product2 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Product 2").Value,
            Description.Create("Description").Value,
            Price.Create(300m).Value,
            sellerId,
            sellerInfo);

        var product3 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Product 3").Value,
            Description.Create("Description").Value,
            Price.Create(200m).Value,
            sellerId,
            sellerInfo);

        _dbContext.Products.AddRange(product1, product2, product3);
        await _dbContext.SaveChangesAsync();

        // Act
        var (products, totalCount) = await _repository.GetPagedAsync(
            1,
            10,
            sortBy: "price",
            sortOrder: "desc");

        // Assert
        products.Should().HaveCount(3);
        products[0].Price.Value.Should().Be(300m);
        products[1].Price.Value.Should().Be(200m);
        products[2].Price.Value.Should().Be(100m);
    }

    [Fact]
    public async Task GetPagedAsync_WithSortByCreatedAt_ShouldSortCorrectly()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var sellerInfo = new SellerSnapshot("Test Seller", "https://example.com/avatar.jpg", 0.0, 0);
        var price = Price.Create(100m).Value;

        var product1 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Product 1").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);
        await Task.Delay(10); // Ensure different timestamps

        var product2 = Product.CreateProduct(
            Guid.NewGuid(),
            Title.Create("Product 2").Value,
            Description.Create("Description").Value,
            price,
            sellerId,
            sellerInfo);

        _dbContext.Products.AddRange(product1, product2);
        await _dbContext.SaveChangesAsync();

        // Act
        var (products, totalCount) = await _repository.GetPagedAsync(
            1,
            10,
            sortBy: "createdat",
            sortOrder: "desc");

        // Assert
        products.Should().HaveCount(2);
        products[0].CreatedAt.Should().BeAfter(products[1].CreatedAt);
    }

    [Fact]
    public async Task GetPagedAsync_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var sellerInfo = new SellerSnapshot("Test Seller", "https://example.com/avatar.jpg", 0.0, 0);
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
            _dbContext.Products.Add(product);
        }

        await _dbContext.SaveChangesAsync();

        // Act
        var (products, totalCount) = await _repository.GetPagedAsync(
            2,
            5);

        // Assert
        products.Should().HaveCount(5);
        totalCount.Should().Be(15);
    }
}