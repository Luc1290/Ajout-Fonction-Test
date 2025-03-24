using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace P3AddNewFunctionalityDotNetCore.Models.ViewModels
{
    public class ProductViewModel
    {
        [BindNever]
        public int Id { get; set; }

        [Required(ErrorMessage = "MissingName")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Details { get; set; }

        // TODO Data annotations présente ici pour la validation des données

        [Required(ErrorMessage = "MissingQuantity")]
        [RegularExpression(@"^\d+$", ErrorMessage = "StockNotAnInteger")]
        [Range(1, int.MaxValue, ErrorMessage = "StockNotGreaterThanZero")]
        public string Stock { get; set; }

        [Required(ErrorMessage = "MissingPrice")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "PriceNotANumber")]
        [Range(0.01, double.MaxValue, ErrorMessage = "PriceNotGreaterThanZero")]
        public string Price { get; set; }
    }
}