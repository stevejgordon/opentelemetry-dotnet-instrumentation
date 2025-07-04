// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

namespace OpenTelemetry.AutoInstrumentation.Instrumentations.SystemDataSqlClient;

/// <summary>
/// Constants for System.Data.SqlClient instrumentation.
/// </summary>
internal static class SqlClientConstants
{
    public const string SystemDataSqlClientByteCodeIntegrationName = "SystemDataSqlClient";
    public const string SystemDataAssemblyName = "System.Data";
    public const string SystemDataSqlCommandTypeName = "System.Data.SqlClient.SqlCommand";
    public const string SqlDataReaderTypeName = "System.Data.SqlClient.SqlDataReader";
    public const string ExecuteReaderMethodName = "ExecuteReader";
    public const string ExecuteReaderAsyncMethodName = "ExecuteReaderAsync";
    public const string CommandBehaviorTypeName = "System.Data.CommandBehavior";
    public const string CancellationTokenTypeName = "System.Threading.CancellationToken";
    public const string SystemDataMinimumVersion = "4.0.0";
    public const string SystemDataMaximumVersion = "4.*.*";
    public const string ActivitySourceName = "OpenTelemetry.AutoInstrumentation.SystemDataSqlClient";
    public const string DatabaseSystem = "microsoft.sql_server";
}
