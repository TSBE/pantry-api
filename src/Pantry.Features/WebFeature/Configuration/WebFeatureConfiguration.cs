namespace Pantry.Features.WebFeature.Configuration;

/// <summary>
/// The web feature settings.
/// </summary>
public class WebFeatureConfiguration
{
    /// <summary>
    /// Name of the configuration section.
    /// </summary>
    public static string Name => "WebFeature";

    /// <summary>
    /// Dummy value.
    /// </summary>
    public int Dummy { get; set; } = 5;
}
