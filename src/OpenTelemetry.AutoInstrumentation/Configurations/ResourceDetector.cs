// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

namespace OpenTelemetry.AutoInstrumentation.Configurations;

/// <summary>
/// Enum representing supported resource detectors.
/// </summary>
internal enum ResourceDetector
{
#if NET8_0_OR_GREATER
    /// <summary>
    /// Container resource detector.
    /// </summary>
    Container = 0,
#endif

    /// <summary>
    /// Azure App Service resource detector.
    /// </summary>
    AzureAppService = 1,

    /// <summary>
    /// Process Runtime resource detector.
    /// </summary>
    ProcessRuntime = 2,

    /// <summary>
    /// Process resource detector.
    /// </summary>
    Process = 3,

    /// <summary>
    /// Host resource detector.
    /// </summary>
    Host = 4,

    /// <summary>
    /// Operating System resource detector.
    /// </summary>
    OperatingSystem = 5,
}
