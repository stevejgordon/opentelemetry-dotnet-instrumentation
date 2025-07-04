// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace OpenTelemetry.AutoInstrumentation.Instrumentations.SystemDataSqlClient;

/// <summary>
/// SqlClient instrumentation
/// </summary>
internal static class SqlClientInstrumentation
{
    private static readonly ActivitySource ActivitySource = new(SqlClientConstants.ActivitySourceName);

    private static readonly Regex ServerRegex = new(
        @"(?:Server|Data Source)\s*=\s*([^\s,;\\]+)(?:\\[^\s,;]*)?(?:,(\d+))?",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Regex to extract SQL operation from command text
    private static readonly Regex SqlOperationRegex = new(
        @"^\s*(SELECT|INSERT|UPDATE|DELETE|EXEC|EXECUTE|WITH|MERGE|CREATE|ALTER|DROP|TRUNCATE|BULK)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static Activity? StartDatabaseActivity(IDbCommand? sqlCommand, string operationType)
    {
        if (sqlCommand == null)
        {
            AutoInstrumentationEventSource.Log.Information("sqlCommand null");
            return null;
        }

        AutoInstrumentationEventSource.Log.Information("sqlCommand not null");

        var database = sqlCommand.Connection?.Database ?? string.Empty;

        var operationName = string.Empty;
        var spanName = string.Empty;

        if (sqlCommand.CommandType == CommandType.Text)
        {
            var commandText = sqlCommand.CommandText ?? string.Empty;
            AutoInstrumentationEventSource.Log.Information($"commandText = {commandText}");

            // TODO - This is overly simplistic and should be improved to meet semantic conventions
            // The main thing missing is statements against multiple databases, which would require a more complex parsing logic.
            operationName = ExtractOperationName(commandText) ?? operationType;
            spanName = database.Length > 0 ? $"{operationName} {database}" : operationName;
        }
        else if (sqlCommand.CommandType == CommandType.StoredProcedure)
        {
            spanName = sqlCommand.CommandText;
        }

        AutoInstrumentationEventSource.Log.Information($"commandText = {operationName}");

        // TODO - Apply semantic conventions for database spans

        var activity = ActivitySource.StartActivity(spanName, ActivityKind.Client);

        if (activity == null)
        {
            AutoInstrumentationEventSource.Log.Information("activity is null");
            return null;
        }

        if (!activity.IsAllDataRequested)
        {
            return activity;
        }

        // Set database attributes
        activity.SetTag("db.system.name", SqlClientConstants.DatabaseSystem);

        if (database.Length > 0)
        {
            activity.SetTag("db.collection.name", database);
            activity.SetTag("db.query.summary", spanName);
        }

        if (sqlCommand.Connection is IDbConnection dbConnection)
        {
            var match = ServerRegex.Match(dbConnection.ConnectionString);
            if (match.Success)
            {
                var address = match.Groups[1].Value;
                var port = match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : (int?)null;
                activity.SetTag("server.address", address);
                if (port.HasValue)
                {
                    activity.SetTag("server.port", port.Value);
                }
            }
        }

        if (!string.IsNullOrEmpty(sqlCommand.CommandText) && sqlCommand.CommandType == CommandType.Text)
        {
            activity.SetTag("db.statement", sqlCommand.CommandText); // SET instead of db.query.text for now so that this is marked as DB span type
        }
        else if (!string.IsNullOrEmpty(sqlCommand.CommandText) && sqlCommand.CommandType == CommandType.StoredProcedure)
        {
            activity.SetTag("db.stored_procedure.name", sqlCommand.CommandText);
            activity.SetTag("db.statement", $"EXECUTE {sqlCommand.CommandText}"); // SET instead of db.query.text for now so that this is marked as DB span type
        }

        activity.SetTag("db.operation.name", sqlCommand.CommandType == CommandType.Text ? operationName : "EXEC");

        return activity;
    }

    public static void StopDatabaseActivity(Activity? activity, Exception? exception)
    {
        if (activity == null)
        {
            AutoInstrumentationEventSource.Log.Information("activity is null");
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
}
