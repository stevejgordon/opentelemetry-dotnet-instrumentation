// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using OpenTelemetry.AutoInstrumentation.DuckTyping;

namespace OpenTelemetry.AutoInstrumentation.Instrumentations.SystemDataSqlClient.DuckTypes;

/// <summary>
/// Duck typing interface for SqlCommand
/// </summary>
internal interface ISqlCommand : IDuckType
{
    /// <summary>
    /// Gets the text command to run against the data source
    /// </summary>
    string? CommandText { get; }

    /// <summary>
    /// Gets the SqlConnection used by this instance of the SqlCommand
    /// </summary>
    object? Connection { get; }
}
