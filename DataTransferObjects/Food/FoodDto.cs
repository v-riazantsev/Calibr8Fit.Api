namespace Calibr8Fit.Api.DataTransferObjects.Food
{
    public record FoodDto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }

        // Basic Nutritional Information (per 100g)
        public required float CaloricValue { get; init; }

        // Macronutrients (in grams per 100g)
        public required float Fat { get; init; }
        public required float SaturatedFats { get; init; }
        public required float MonounsaturatedFats { get; init; }
        public required float PolyunsaturatedFats { get; init; }
        public required float Carbohydrates { get; init; }
        public required float Sugars { get; init; }
        public required float Protein { get; init; }
        public required float DietaryFiber { get; init; }
        public required float Water { get; init; }

        // Other Important Nutrients
        public required float Cholesterol { get; init; }
        public required float Sodium { get; init; }

        // Vitamins
        public required float VitaminA { get; init; }
        public required float VitaminB1Thiamine { get; init; }
        public required float VitaminB11FolicAcid { get; init; }
        public required float VitaminB12 { get; init; }
        public required float VitaminB2Riboflavin { get; init; }
        public required float VitaminB3Niacin { get; init; }
        public required float VitaminB5PantothenicAcid { get; init; }
        public required float VitaminB6 { get; init; }
        public required float VitaminC { get; init; }
        public required float VitaminD { get; init; }
        public required float VitaminE { get; init; }
        public required float VitaminK { get; init; }

        // Minerals
        public required float Calcium { get; init; }
        public required float Copper { get; init; }
        public required float Iron { get; init; }
        public required float Magnesium { get; init; }
        public required float Manganese { get; init; }
        public required float Phosphorus { get; init; }
        public required float Potassium { get; init; }
        public required float Selenium { get; init; }
        public required float Zinc { get; init; }

        // Nutritional Quality Metric
        public required float NutritionDensity { get; init; }
    }
}
