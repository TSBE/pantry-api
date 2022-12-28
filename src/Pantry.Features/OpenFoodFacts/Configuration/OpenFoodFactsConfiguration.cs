using System;
using System.ComponentModel.DataAnnotations;

namespace Pantry.Features.OpenFoodFacts.Configuration;

/// <summary>
/// The open food facts settings.
/// </summary>
public class OpenFoodFactsConfiguration
{
    /// <summary>
    /// Name of the configuration section.
    /// </summary>
    public static string Name => "OpenFoodFacts";

    /// <summary>
    /// HttpClient RequestTimeout in milliseconds.
    /// </summary>
    public int TimeoutInMilliseconds { get; set; } = 30_000;

    /// <summary>
    /// Base url.
    /// </summary>
    [Required]
    public Uri? BaseUrl { get; set; }
}
