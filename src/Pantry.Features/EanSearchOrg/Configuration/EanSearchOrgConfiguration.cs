using System;
using System.ComponentModel.DataAnnotations;

namespace Pantry.Features.EanSearchOrg.Configuration;

/// <summary>
/// The ean-search.org settings.
/// </summary>
public class EanSearchOrgConfiguration
{
    /// <summary>
    /// Name of the configuration section.
    /// </summary>
    public static string Name => "EanSearchOrg";

    /// <summary>
    /// HttpClient RequestTimeout in milliseconds.
    /// </summary>
    public int TimeoutInMilliseconds { get; set; } = 30_000;

    /// <summary>
    /// Base url.
    /// </summary>
    [Required]
    public Uri? BaseUrl { get; set; }

    /// <summary>
    /// Token.
    /// </summary>
    [Required]
    public string Token { get; set; } = string.Empty;
}
