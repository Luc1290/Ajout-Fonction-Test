using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductModelValidationTests
    {
        private List<ValidationResult> ValidateModel(ProductViewModel model)
        {
            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        [Fact]
        public void ValidProduct_ShouldBeValid()
        {
            var product = new ProductViewModel
            {
                Name = "Valid Product",
                Price = "10.99",
                Stock = "5",
                Description = "Description",
                Details = "Details"
            };

            var results = ValidateModel(product);
            Assert.Empty(results);
        }

        [Fact]
        public void MissingName_ShouldReturnError()
        {
            var product = new ProductViewModel
            {
                Name = "",
                Price = "10.99",
                Stock = "5"
            };

            var results = ValidateModel(product);
            Assert.Contains(results, r => r.ErrorMessage == "MissingName");
        }

        [Fact]
        public void MissingPrice_ShouldReturnError()
        {
            var product = new ProductViewModel
            {
                Name = "Test Product",
                Price = "",
                Stock = "5"
            };

            var results = ValidateModel(product);
            Assert.Contains(results, r => r.ErrorMessage == "MissingPrice");
        }

        [Fact]
        public void InvalidPriceFormat_ShouldReturnError()
        {
            var product = new ProductViewModel
            {
                Name = "Test Product",
                Price = "abc",
                Stock = "5"
            };

            var results = ValidateModel(product);
            Assert.Contains(results, r => r.ErrorMessage == "PriceNotANumber");
        }

        [Fact]
        public void PriceLessThanOrEqualToZero_ShouldReturnError()
        {
            var product = new ProductViewModel
            {
                Name = "Test Product",
                Price = "0",
                Stock = "5"
            };

            var results = ValidateModel(product);
            Assert.Contains(results, r => r.ErrorMessage == "PriceNotGreaterThanZero");
        }

        [Fact]
        public void MissingStock_ShouldReturnError()
        {
            var product = new ProductViewModel
            {
                Name = "Test Product",
                Price = "10.99",
                Stock = ""
            };

            var results = ValidateModel(product);
            Assert.Contains(results, r => r.ErrorMessage == "MissingQuantity");
        }

        [Fact]
        public void InvalidStockFormat_ShouldReturnError()
        {
            var product = new ProductViewModel
            {
                Name = "Test Product",
                Price = "10.99",
                Stock = "notanumber"
            };

            var results = ValidateModel(product);
            Assert.Contains(results, r => r.ErrorMessage == "StockNotAnInteger");
        }

        [Fact]
        public void StockLessThanOrEqualToZero_ShouldReturnError()
        {
            var product = new ProductViewModel
            {
                Name = "Test Product",
                Price = "10.99",
                Stock = "0"
            };

            var results = ValidateModel(product);
            Assert.Contains(results, r => r.ErrorMessage == "StockNotGreaterThanZero");
        }

        [Fact]
        public void MultipleErrors_ShouldReturnAll()
        {
            var product = new ProductViewModel
            {
                Name = "",
                Price = "-10",
                Stock = "abc"
            };

            var results = ValidateModel(product);
            
            Assert.Contains(results, r => r.ErrorMessage == "MissingName");
            Assert.Contains(results, r => r.ErrorMessage == "PriceNotGreaterThanZero");
            Assert.Contains(results, r => r.ErrorMessage == "StockNotAnInteger");
        }
    }
}