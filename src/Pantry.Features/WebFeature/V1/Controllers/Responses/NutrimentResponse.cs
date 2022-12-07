namespace Pantry.Features.WebFeature.V1.Controllers.Responses;

public class NutrimentResponse
{
    public double Name { get; set; }

    public string? Unit { get; set; }

    public double Value { get; set; }

    public double ValueFor100g { get; set; }
}
