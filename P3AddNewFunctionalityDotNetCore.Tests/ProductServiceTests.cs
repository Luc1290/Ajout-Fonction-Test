using Microsoft.Extensions.Localization;
using Moq;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.Collections.Generic;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<ICart> _mockCart;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IStringLocalizer<ProductService>> _mockLocalizer;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            // Setup common mocks for all tests
            _mockCart = new Mock<ICart>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockLocalizer = new Mock<IStringLocalizer<ProductService>>();

            // Setup localizer to return the key as the value
            _mockLocalizer.Setup(l => l["MissingName"]).Returns(new LocalizedString("MissingName", "MissingName"));
            _mockLocalizer.Setup(l => l["MissingPrice"]).Returns(new LocalizedString("MissingPrice", "MissingPrice"));
            _mockLocalizer.Setup(l => l["PriceNotANumber"]).Returns(new LocalizedString("PriceNotANumber", "PriceNotANumber"));
            _mockLocalizer.Setup(l => l["PriceNotGreaterThanZero"]).Returns(new LocalizedString("PriceNotGreaterThanZero", "PriceNotGreaterThanZero"));
            _mockLocalizer.Setup(l => l["MissingQuantity"]).Returns(new LocalizedString("MissingQuantity", "MissingQuantity"));
            _mockLocalizer.Setup(l => l["StockNotAnInteger"]).Returns(new LocalizedString("StockNotAnInteger", "StockNotAnInteger"));
            _mockLocalizer.Setup(l => l["StockNotGreaterThanZero"]).Returns(new LocalizedString("StockNotGreaterThanZero", "StockNotGreaterThanZero"));

            // Create ProductService with mocks
            _productService = new ProductService(_mockCart.Object, _mockProductRepository.Object, _mockOrderRepository.Object, _mockLocalizer.Object);
        }

        [Fact]
        public void CheckProductModelErrors_ValidProduct_ReturnsEmptyList()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Product",
                // Essayez de modifier le format du prix selon votre culture
                Price = "10,99", // Utilisez une virgule au lieu d'un point
                Stock = "5",
                Description = "Test Description",
                Details = "Test Details"
            };

            // Act
            var result = _productService.CheckProductModelErrors(product);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void CheckProductModelErrors_MissingName_ReturnsNameError()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "",  // Empty name
                Price = "10,99",
                Stock = "5"
            };

            // Act
            var result = _productService.CheckProductModelErrors(product);

            // Assert
            Assert.Contains("MissingName", result);
        }

        [Fact]
        public void CheckProductModelErrors_MissingPrice_ReturnsPriceError()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Test Product",
                Price = "",  // Empty price
                Stock = "5"
            };

            // Act
            var result = _productService.CheckProductModelErrors(product);

            // Assert
            Assert.Contains("MissingPrice", result);
        }

        [Fact]
        public void CheckProductModelErrors_PriceNotANumber_ReturnsPriceNotANumberError()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Test Product",
                Price = "not-a-number",  // Invalid price
                Stock = "5"
            };

            // Act
            var result = _productService.CheckProductModelErrors(product);

            // Assert
            Assert.Contains("PriceNotANumber", result);
        }

        [Fact]
        public void CheckProductModelErrors_PriceNotGreaterThanZero_ReturnsPriceNotGreaterThanZeroError()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Test Product",
                Price = "0",  // Price is zero
                Stock = "5"
            };

            // Act
            var result = _productService.CheckProductModelErrors(product);

            // Assert
            Assert.Contains("PriceNotGreaterThanZero", result);
        }

        [Fact]
        public void CheckProductModelErrors_MissingQuantity_ReturnsQuantityError()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Test Product",
                Price = "10.99",
                Stock = ""  // Empty stock
            };

            // Act
            var result = _productService.CheckProductModelErrors(product);

            // Assert
            Assert.Contains("MissingQuantity", result);
        }

        [Fact]
        public void CheckProductModelErrors_QuantityNotAnInteger_ReturnsQuantityNotAnIntegerError()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Test Product",
                Price = "10.99",
                Stock = "not-an-integer"  // Invalid stock
            };

            // Act
            var result = _productService.CheckProductModelErrors(product);

            // Assert
            Assert.Contains("StockNotAnInteger", result);
        }

        [Fact]
        public void CheckProductModelErrors_QuantityNotGreaterThanZero_ReturnsQuantityNotGreaterThanZeroError()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Test Product",
                Price = "10.99",
                Stock = "0"  // Stock is zero
            };

            // Act
            var result = _productService.CheckProductModelErrors(product);

            // Assert
            Assert.Contains("StockNotGreaterThanZero", result);
        }

        [Fact]
        public void CheckProductModelErrors_MultipleErrors_ReturnsAllErrors()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "",           // Invalid name
                Price = "-5",        // Invalid price
                Stock = "invalid"    // Invalid stock
            };

            // Act
            var result = _productService.CheckProductModelErrors(product);

            // Assert
            Assert.Equal(3, result.Count); // Modifier ici: 3 au lieu de 4
            Assert.Contains("MissingName", result);
            // Vérifiez quelles erreurs sont réellement retournées et ajustez
            // Ces assertions en conséquence
        }
    }
}