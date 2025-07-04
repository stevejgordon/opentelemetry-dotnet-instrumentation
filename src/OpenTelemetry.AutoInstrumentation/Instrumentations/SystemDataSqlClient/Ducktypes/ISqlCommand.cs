// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

namespace OpenTelemetry.AutoInstrumentation.Instrumentations.SystemDataSqlClient.DuckTypes;

/// <summary>
/// Duck typing interface for SqlCommand
/// </summary>
internal interface ISqlCommand
{
    /// <summary>
    /// Gets the text command to run against the data source
    /// </summary>
    string CommandText { get; }

    /// <summary>
    /// Gets or sets the connection to the data source
    /// </summary>
    ISqlConnection Connection { get; set; }
}
