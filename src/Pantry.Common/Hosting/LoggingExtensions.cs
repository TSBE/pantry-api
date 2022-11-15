using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Pantry.Common.Hosting;

/// <summary>
///     Provides Extensions for the <see cref="ILogger" />.
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    ///     Logs several information about the runtime and the environment.
    ///     This method is supposed to be called on application startup, in the Configure method of the startup-class.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:Parameter should not span multiple lines", Justification = "Better readability.")]
    public static void LogRuntimeAndEnvironmentInformation(this ILogger logger)
    {
        if (logger == null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        ThreadPool.GetMinThreads(out var minWorkerThreads, out var minIoThreads);
        ThreadPool.GetMaxThreads(out var maxWorkerThreads, out var maxIoThreads);
        var processorCount = Environment.ProcessorCount;
        var clrVersion = Environment.Version;
        var dotnetFrameworkVersion = RuntimeInformation.FrameworkDescription;
        var osVersion = Environment.OSVersion.ToString();

        logger.LogInformation(
            new EventId(1, "StartupInformation"),
            "ProcessorCount: {ProcessorCount}, " +
            "MinWorkerThreads: {MinWorkerThreads}, " +
            "MinIoThreads: {MinIoThreads}, " +
            "MaxWorkerThreads: {MaxWorkerThreads}, " +
            "MaxIoThreads: {MaxIoThreads}, " +
            "ClrVersion: {ClrVersion}, " +
            "DotnetFrameworkVersion: {DotnetFrameworkVersion}, " +
            "OsVersion: {OsVersion}",
            processorCount,
            minWorkerThreads,
            minIoThreads,
            maxWorkerThreads,
            maxIoThreads,
            clrVersion,
            dotnetFrameworkVersion,
            osVersion);
    }
}
