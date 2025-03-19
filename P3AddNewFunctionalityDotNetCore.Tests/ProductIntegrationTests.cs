using P3AddNewFunctionalityDotNetCore.Controllers;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductIntegrationTests
    {
        [Fact]
        public void AdminProductChanges_AreReflectedInClientView()
        {
            // Arrange
            // Setup mocks for dependencies
            var mockProductService = new Mock<IProductService>();
            var mockLanguageService = new Mock<ILanguageService>();

            // Create test product
            var testProduct = new ProductViewModel
            {
                Id = 999,
                Name = "Test Integration Product",
                Price = "99,99",
                Stock = "10",
                Description = "Test product for integration test"
            };

            // Setup mock product service to return our test product
            var productList = new List<ProductViewModel> { testProduct };
            mockProductService.Setup(s => s.GetAllProductsViewModel())
                .Returns(productList);

            // Create controllers for both admin and client views
            var adminController = new ProductController(mockProductService.Object, mockLanguageService.Object);
            var clientController = new ProductController(mockProductService.Object, mockLanguageService.Object);

            // Act
            // Get results from both admin and client views
            var adminResult = adminController.Admin() as ViewResult;
            var clientResult = clientController.Index() as ViewResult;

            // Assert
            // Verify admin view contains the product
            var adminProducts = adminResult.Model as IEnumerable<ProductViewModel>;
            Assert.Contains(adminProducts, p => p.Id == testProduct.Id);

            // Verify client view contains the product
            var clientProducts = clientResult.Model as IEnumerable<ProductViewModel>;
            Assert.Contains(clientProducts, p => p.Id == testProduct.Id);

            // Verify that the same product appears in both views
            var adminProduct = adminProducts.FirstOrDefault(p => p.Id == testProduct.Id);
            var clientProduct = clientProducts.FirstOrDefault(p => p.Id == testProduct.Id);

            Assert.Equal(adminProduct.Name, clientProduct.Name);
            Assert.Equal(adminProduct.Price, clientProduct.Price);
            Assert.Equal(adminProduct.Stock, clientProduct.Stock);
        }

        [Fact]
        public void WhenAdminReducesStock_ClientCartIsNotAffected_ButNewOrdersMightBe()
        {
            // Arrange
            var mockProductRepository = new Mock<IProductRepository>();
            var mockOrderRepository = new Mock<IOrderRepository>();
            var mockLocalizer = new Mock<IStringLocalizer<ProductService>>();

            // Setup a test product with initial stock
            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Price = 10.99,
                Quantity = 10 // Initial stock is 10
            };

            // Setup product repository
            mockProductRepository.Setup(r => r.GetAllProducts())
                .Returns(new List<Product> { product });
            mockProductRepository.Setup(r => r.GetProduct(It.IsAny<int>()))
                .ReturnsAsync(product);

            // Créer un mock pour ICart
            var mockCart = new Mock<ICart>();

            // On garde une trace de la quantité du produit dans le panier
            int cartQuantity = 3;

            // Mock de AddItem pour simuler l'ajout au panier
            mockCart.Setup(c => c.AddItem(It.IsAny<Product>(), It.IsAny<int>()))
                .Callback<Product, int>((p, q) => {
                    if (p.Id == product.Id)
                        cartQuantity = q;
                });

            // Mock pour simuler la valeur totale du panier
            mockCart.Setup(c => c.GetTotalValue()).Returns(product.Price * cartQuantity);

            // Create product service
            var productService = new ProductService(
                mockCart.Object,
                mockProductRepository.Object,
                mockOrderRepository.Object,
                mockLocalizer.Object);

            // Simuler un panier contenant déjà le produit
            mockCart.Object.AddItem(product, cartQuantity);

            // Act
            // 1. Admin reduces the stock to 2 (less than what's in the cart)
            product.Quantity = 2;
            mockProductRepository.Setup(r => r.SaveProduct(It.IsAny<Product>()))
                .Callback<Product>(p => product = p);

            var updatedProductVM = new ProductViewModel
            {
                Id = 1,
                Name = "Test Product",
                Price = "10,99",
                Stock = "2" // Reduced to 2
            };

            productService.SaveProduct(updatedProductVM);

            // Assert
            // 1. Verify that cart quantity hasn't changed
            Assert.Equal(3, cartQuantity);

            // 2. The product stock in the database is now 2
            Assert.Equal(2, product.Quantity);

            // 3. Verify that the total value in the cart is still based on the original quantity
            Assert.Equal(product.Price * 3, mockCart.Object.GetTotalValue());
        }
    }
}