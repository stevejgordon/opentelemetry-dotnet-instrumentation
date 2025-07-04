// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using OpenTelemetry.AutoInstrumentation.DuckTyping;

namespace OpenTelemetry.AutoInstrumentation.Instrumentations.SystemDataSqlClient.DuckTypes;

/// <summary>
/// Duck typing interface for SqlConnection
/// </summary>
internal interface ISqlConnection
{
    /// <summary>
    /// Gets the name of the current database or the database to be used after a connection is opened
    /// </summary>
    string Database { get; }
}
