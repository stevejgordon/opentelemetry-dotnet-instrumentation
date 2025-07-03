// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using OpenTelemetry.AutoInstrumentation.DuckTyping;

namespace OpenTelemetry.AutoInstrumentation.Instrumentations.SystemDataSqlClient.DuckTypes;

/// <summary>
/// Duck typing interface for SqlConnection
/// </summary>
internal interface ISqlConnection : IDuckType
{
    /// <summary>
    /// Gets the name of the current database or the database to be used after a connection is opened
    /// </summary>
    string? Database { get; }

    /// <summary>
    /// Gets the name of the instance of SQL Server to which to connect
    /// </summary>
    string? DataSource { get; }
}
