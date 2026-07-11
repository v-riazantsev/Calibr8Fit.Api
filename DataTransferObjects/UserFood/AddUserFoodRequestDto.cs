using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.UserFood
{
    public record AddUserFoodRequestDto
    {
        public Guid Id { get; init; } // Optional, will be generated if not provided
        [Required]
        public required string Name { get; init; }
        [Required]
        public required float CaloricValue { get; init; }
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
        [Required]
        public required float Cholesterol { get; init; }
        [Required]
        public required float Sodium { get; init; }
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
        [Required]
        public required float NutritionDensity { get; init; }
        public DateTime ModifiedAt { get; init; } = DateTime.UtcNow; // Default to current time if not specified
        public bool Deleted { get; init; } = false; // Default to false if not specified
    }
}
