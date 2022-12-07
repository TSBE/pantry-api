#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System.Text.Json.Serialization;

namespace Pantry.Core.Models.OpenFoodFacts;

public class Nutriments
{
    // <summary>
    // Energy kcal
    // </summary>

    [JsonPropertyName("energykcal")]
    public double Energykcal { get; set; }

    [JsonPropertyName("energykcal_100g")]
    public double Energykcal100g { get; set; }

    [JsonPropertyName("energykcal_unit")]
    public string EnergykcalUnit { get; set; }

    [JsonPropertyName("energykcal_value")]
    public double EnergykcalValue { get; set; }

    [JsonPropertyName("energykcal_value_computed")]
    public double EnergykcalValueComputed { get; set; }

    // <summary>
    // Energy
    // </summary>

    [JsonPropertyName("energy")]
    public double Energy { get; set; }

    [JsonPropertyName("energy_100g")]
    public double Energy100g { get; set; }

    [JsonPropertyName("energy_unit")]
    public string EnergyUnit { get; set; }

    [JsonPropertyName("energy_value")]
    public double EnergyValue { get; set; }

    // <summary>
    // Fat
    // </summary>

    [JsonPropertyName("fat")]
    public double Fat { get; set; }

    [JsonPropertyName("fat_100g")]
    public double Fat100g { get; set; }

    [JsonPropertyName("fat_unit")]
    public string FatUnit { get; set; }

    [JsonPropertyName("fat_value")]
    public double FatValue { get; set; }

    // <summary>
    // Saturaded fat
    // </summary>

    [JsonPropertyName("saturatedfat")]
    public double Saturatedfat { get; set; }

    [JsonPropertyName("saturatedfat_100g")]
    public double Saturatedfat100g { get; set; }

    [JsonPropertyName("saturatedfat_unit")]
    public string SaturatedfatUnit { get; set; }

    [JsonPropertyName("saturatedfat_value")]
    public double SaturatedfatValue { get; set; }

    // <summary>
    // Carbohydrates
    // </summary>

    [JsonPropertyName("carbohydrates")]
    public double Carbohydrates { get; set; }

    [JsonPropertyName("carbohydrates_100g")]
    public double Carbohydrates100g { get; set; }

    [JsonPropertyName("carbohydrates_unit")]
    public string CarbohydratesUnit { get; set; }

    [JsonPropertyName("carbohydrates_value")]
    public double CarbohydratesValue { get; set; }

    // <summary>
    // Sugar
    // </summary>

    [JsonPropertyName("sugars")]
    public double Sugars { get; set; }

    [JsonPropertyName("sugars_100g")]
    public double Sugars100g { get; set; }

    [JsonPropertyName("sugars_unit")]
    public string SugarsUnit { get; set; }

    [JsonPropertyName("sugars_value")]
    public double SugarsValue { get; set; }

    // <summary>
    // Fiber
    // </summary>

    [JsonPropertyName("fiber")]
    public double Fiber { get; set; }

    [JsonPropertyName("fiber_100g")]
    public double Fiber100g { get; set; }

    [JsonPropertyName("fiber_unit")]
    public string FiberUnit { get; set; }

    [JsonPropertyName("fiber_value")]
    public double FiberValue { get; set; }

    // <summary>
    // Protein
    // </summary>

    [JsonPropertyName("proteins")]
    public double Proteins { get; set; }

    [JsonPropertyName("proteins_100g")]
    public double Proteins100g { get; set; }

    [JsonPropertyName("proteins_unit")]
    public string ProteinsUnit { get; set; }

    [JsonPropertyName("proteins_value")]
    public double ProteinsValue { get; set; }

    // <summary>
    // Salt
    // </summary>

    [JsonPropertyName("salt")]
    public double Salt { get; set; }

    [JsonPropertyName("salt_100g")]
    public double Salt100g { get; set; }

    [JsonPropertyName("salt_unit")]
    public string SaltUnit { get; set; }

    [JsonPropertyName("salt_value")]
    public double SaltValue { get; set; }

    // <summary>
    // Sodium
    // </summary>

    [JsonPropertyName("sodium")]
    public double Sodium { get; set; }

    [JsonPropertyName("sodium_100g")]
    public double Sodium100g { get; set; }

    [JsonPropertyName("sodium_unit")]
    public string SodiumUnit { get; set; }

    [JsonPropertyName("sodium_value")]
    public double SodiumValue { get; set; }
}
