// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

#if NETFRAMEWORK
using System.Diagnostics;
using OpenTelemetry.AutoInstrumentation.CallTarget;
using OpenTelemetry.AutoInstrumentation.Instrumentations.SystemDataSqlClient.DuckTypes;
using OpenTelemetry.AutoInstrumentation.Util;

namespace OpenTelemetry.AutoInstrumentation.Instrumentations.SystemDataSqlClient.Integrations;

/// <summary>
/// SqlCommand ExecuteReader integration
/// </summary>
// System.Data (.NET Framework) - ExecuteReader()
[InstrumentMethod(
    assemblyName: SqlClientConstants.SystemDataAssemblyName,
    typeName: SqlClientConstants.SystemDataSqlCommandTypeName,
    methodName: SqlClientConstants.ExecuteReaderMethodName,
    returnTypeName: SqlClientConstants.SqlDataReaderTypeName,
    parameterTypeNames: [],
    minimumVersion: SqlClientConstants.SystemDataMinimumVersion,
    maximumVersion: SqlClientConstants.SystemDataMaximumVersion,
    integrationName: SqlClientConstants.SystemDataSqlClientByteCodeIntegrationName,
    type: InstrumentationType.Trace)]
public static class SqlCommandExecuteReader
{
    /// <summary>
    /// OnMethodBegin callback
    /// </summary>
    /// <typeparam name="TTarget">Type of the target</typeparam>
    /// <param name="instance">Instance value, aka `this` of the instrumented method.</param>
    /// <returns>Calltarget state value</returns>
    internal static CallTargetState OnMethodBegin<TTarget>(TTarget instance)
    {
        AutoInstrumentationEventSource.Log.Information("ZZZZ TEST");

        var activity = SqlClientInstrumentation.StartDatabaseActivity(instance, "ExecuteReader");

        if (activity is null)
        {
            AutoInstrumentationEventSource.Log.Information("activity is null");
        }

        return activity is not null ? new CallTargetState(activity) : CallTargetState.GetDefault();
    }

    /// <summary>
    /// OnMethodEnd callback
    /// </summary>
    /// <typeparam name="TTarget">Type of the target</typeparam>
    /// <typeparam name="TReturn">Type of the return value</typeparam>
    /// <param name="instance">Instance value, aka `this` of the instrumented method.</param>
    /// <param name="returnValue">Return value</param>
    /// <param name="exception">Exception instance in case the original code threw an exception.</param>
    /// <param name="state">Calltarget state value</param>
    /// <returns>A response value, in an async scenario will be T of Task of T</returns>
    internal static CallTargetReturn<TReturn> OnMethodEnd<TTarget, TReturn>(TTarget instance, TReturn returnValue, Exception? exception, in CallTargetState state)
    {
        AutoInstrumentationEventSource.Log.Information("ZZZZ END TEST");
        AutoInstrumentationEventSource.Log.Information(state.ToString());

        var activity = state.Activity;

        if (activity is null)
        {
            AutoInstrumentationEventSource.Log.Information("activity is null");
            return new CallTargetReturn<TReturn>(returnValue);
        }

        SqlClientInstrumentation.StopDatabaseActivity(activity, exception);
        return new CallTargetReturn<TReturn>(returnValue);
    }
}
#endif
