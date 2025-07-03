// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using System.Diagnostics;
using System.Text.RegularExpressions;
using OpenTelemetry.AutoInstrumentation.DuckTyping;
using OpenTelemetry.AutoInstrumentation.Instrumentations.SystemDataSqlClient.DuckTypes;

namespace OpenTelemetry.AutoInstrumentation.Instrumentations.SystemDataSqlClient;

/// <summary>
/// SqlClient instrumentation
/// </summary>
internal static class SqlClientInstrumentation
{
    private static readonly ActivitySource ActivitySource = new(SqlClientConstants.ActivitySourceName);

    // Regex to extract SQL operation from command text
    private static readonly Regex SqlOperationRegex = new(
        @"^\s*(SELECT|INSERT|UPDATE|DELETE|EXEC|EXECUTE|WITH|MERGE|CREATE|ALTER|DROP|TRUNCATE|BULK)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static Activity? StartDatabaseActivity(object? sqlCommand, string operationType)
    {
        if (sqlCommand == null)
        {
            return null;
        }

        // Check if source instrumentation is already handling this operation
        // to prevent duplicate spans from bytecode and source instrumentation
        // var currentActivity = Activity.Current;
        // if (currentActivity?.Source.Name == "OpenTelemetry.Instrumentation.SqlClient")
        // {
        //     // Source instrumentation is already active - let it handle the telemetry
        //     return null;
        // }

        if (!sqlCommand.TryDuckCast<ISqlCommand>(out var command))
        {
            return null;
        }

        var commandText = command.CommandText ?? string.Empty;
        var operationName = ExtractOperationName(commandText) ?? operationType;

        // TODO - Apply semantic conventions for database spans

        var activity = ActivitySource.StartActivity($"{operationName} {ExtractDatabaseName(command)}");
        if (activity == null)
        {
            return null;
        }

        // Set database attributes
        activity.SetTag("db.system", SqlClientConstants.DatabaseSystem);

        if (!string.IsNullOrEmpty(commandText))
        {
            activity.SetTag("db.statement", commandText);
        }

        activity.SetTag("db.operation.name", operationName);

        // Extract connection information
        if (command.Connection?.TryDuckCast<ISqlConnection>(out var connection) == true)
        {
            if (!string.IsNullOrEmpty(connection.Database))
            {
                activity.SetTag("db.namespace", connection.Database);
            }

            if (!string.IsNullOrEmpty(connection.DataSource))
            {
                var serverInfo = ParseServerInfo(connection.DataSource!);
                activity.SetTag("server.address", serverInfo.Address);

                if (serverInfo.Port.HasValue)
                {
                    activity.SetTag("server.port", serverInfo.Port.Value);
                }
            }
        }

        return activity;
    }

    public static void StopDatabaseActivity(Activity? activity, Exception? exception)
    {
        if (activity == null)
        {
            return;
        }

        if (exception != null)
        {
            activity.SetStatus(ActivityStatusCode.Error, exception.Message);
            activity.SetTag("error.type", exception.GetType().FullName);
            activity.SetTag("error.message", exception.Message);
        }
        else
        {
            activity.SetStatus(ActivityStatusCode.Ok);
        }

        activity.Stop();
    }

    private static string? ExtractOperationName(string commandText)
    {
        if (string.IsNullOrWhiteSpace(commandText))
        {
            return null;
        }

        var match = SqlOperationRegex.Match(commandText);
        return match.Success ? match.Groups[1].Value.ToUpperInvariant() : null;
    }

    private static string ExtractDatabaseName(ISqlCommand command)
    {
        if (command.Connection?.TryDuckCast<ISqlConnection>(out var connection) == true)
        {
            return connection.Database ?? "unknown";
        }

        return "unknown";
    }

    private static (string Address, int? Port) ParseServerInfo(string dataSource)
    {
        if (string.IsNullOrEmpty(dataSource))
        {
            return ("unknown", null);
        }

        // Handle formats like:
        // - server.domain.com
        // - server.domain.com,1433
        // - server.domain.com\INSTANCE
        // - server.domain.com\INSTANCE,1433

        var parts = dataSource.Split(',');
        var serverPart = parts[0];
        int? port = null;

        if (parts.Length > 1 && int.TryParse(parts[1], out var parsedPort))
        {
            port = parsedPort;
        }

        // Remove instance name if present
        var instanceIndex = serverPart.IndexOf('\\');
        if (instanceIndex >= 0)
        {
            serverPart = serverPart.Substring(0, instanceIndex);
        }

        return (serverPart, port);
    }
}
