using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.Food
{
    public record AddFoodRequestDto
    {
        public Guid Id { get; init; } // Optional, will be generated if not provided
        [Required]
        public required string Name { get; init; }

        // Basic Nutritional Information (per 100g)
        [Required]
        public required float CaloricValue { get; init; }

        // Macronutrients (in grams per 100g)
        [Required]
        public required float Fat { get; init; }
        [Required]
        public required float SaturatedFats { get; init; }
        [Required]
        public required float MonounsaturatedFats { get; init; }
        [Required]
        public required float PolyunsaturatedFats { get; init; }
        [Required]
        public required float Carbohydrates { get; init; }
        [Required]
        public required float Sugars { get; init; }
        [Required]
        public required float Protein { get; init; }
        [Required]
        public required float DietaryFiber { get; init; }
        [Required]
        public required float Water { get; init; }

        // Other Important Nutrients
        [Required]
        public required float Cholesterol { get; init; }
        [Required]
        public required float Sodium { get; init; }

        // Vitamins
        [Required]
        public required float VitaminA { get; init; }
        [Required]
        public required float VitaminB1Thiamine { get; init; }
        [Required]
        public required float VitaminB11FolicAcid { get; init; }
        [Required]
        public required float VitaminB12 { get; init; }
        [Required]
        public required float VitaminB2Riboflavin { get; init; }
        [Required]
        public required float VitaminB3Niacin { get; init; }
        [Required]
        public required float VitaminB5PantothenicAcid { get; init; }
        [Required]
        public required float VitaminB6 { get; init; }
        [Required]
        public required float VitaminC { get; init; }
        [Required]
        public required float VitaminD { get; init; }
        [Required]
        public required float VitaminE { get; init; }
        [Required]
        public required float VitaminK { get; init; }

        // Minerals
        [Required]
        public required float Calcium { get; init; }
        [Required]
        public required float Copper { get; init; }
        [Required]
        public required float Iron { get; init; }
        [Required]
        public required float Magnesium { get; init; }
        [Required]
        public required float Manganese { get; init; }
        [Required]
        public required float Phosphorus { get; init; }
        [Required]
        public required float Potassium { get; init; }
        [Required]
        public required float Selenium { get; init; }
        [Required]
        public required float Zinc { get; init; }

        // Nutritional Quality Metric
        [Required]
        public required float NutritionDensity { get; init; }
    }
}
