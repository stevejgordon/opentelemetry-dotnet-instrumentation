// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using System.Runtime.CompilerServices;
using OpenTelemetry.AutoInstrumentation.Loading;
using OpenTelemetry.AutoInstrumentation.Loading.Initializers;
using OpenTelemetry.AutoInstrumentation.Plugins;

namespace OpenTelemetry.AutoInstrumentation.Configurations;

internal static class DelayedInitialization
{
    internal static class Traces
    {
#if NETFRAMEWORK
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void AddAspNet(LazyInstrumentationLoader lazyInstrumentationLoader, PluginManager pluginManager, TracerSettings tracerSettings)
        {
            new AspNetInitializer(lazyInstrumentationLoader, pluginManager, tracerSettings);
        }
#endif

#if NET8_0_OR_GREATER
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void AddAspNetCore(LazyInstrumentationLoader lazyInstrumentationLoader, PluginManager pluginManager, TracerSettings tracerSettings)
        {
            lazyInstrumentationLoader.Add(new AspNetCoreInitializer(pluginManager, tracerSettings));
        }
#endif

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void AddHttpClient(LazyInstrumentationLoader lazyInstrumentationLoader, PluginManager pluginManager, TracerSettings tracerSettings)
        {
            new HttpClientInitializer(lazyInstrumentationLoader, pluginManager, tracerSettings);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void AddGrpcClient(LazyInstrumentationLoader lazyInstrumentationLoader, PluginManager pluginManager, TracerSettings tracerSettings)
        {
            lazyInstrumentationLoader.Add(new GrpcClientInitializer(pluginManager, tracerSettings));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void AddSqlClient(LazyInstrumentationLoader lazyInstrumentationLoader, PluginManager pluginManager, TracerSettings tracerSettings)
        {
            new SqlClientInitializer(lazyInstrumentationLoader, pluginManager, tracerSettings);
        }

#if NET8_0_OR_GREATER

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void AddEntityFrameworkCore(LazyInstrumentationLoader lazyInstrumentationLoader, PluginManager pluginManager, TracerSettings tracerSettings)
        {
            lazyInstrumentationLoader.Add(new EntityFrameworkCoreInitializer(pluginManager, tracerSettings));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void AddGraphQL(LazyInstrumentationLoader lazyInstrumentationLoader, PluginManager pluginManager, TracerSettings tracerSettings)
        {
            lazyInstrumentationLoader.Add(new GraphQLInitializer(pluginManager, tracerSettings));
        }
#endif

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void AddQuartz(LazyInstrumentationLoader lazyInstrumentationLoader, PluginManager pluginManager)
        {
            lazyInstrumentationLoader.Add(new QuartzInitializer(pluginManager));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void AddOracleMda(LazyInstrumentationLoader lazyInstrumentationLoader, TracerSettings tracerSettings)
        {
            lazyInstrumentationLoader.Add(new OracleMdaInitializer(tracerSettings));
        }
    }

    internal static class Metrics
    {
#if NETFRAMEWORK
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void AddAspNet(LazyInstrumentationLoader lazyInstrumentationLoader, PluginManager pluginManager)
        {
            new AspNetMetricsInitializer(lazyInstrumentationLoader, pluginManager);
        }
#endif

#if NET8_0_OR_GREATER
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void AddAspNetCore(LazyInstrumentationLoader lazyInstrumentationLoader)
        {
            lazyInstrumentationLoader.Add(new AspNetCoreMetricsInitializer());
        }
#endif

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void AddHttpClient(LazyInstrumentationLoader lazyInstrumentationLoader)
        {
            new HttpClientMetricsInitializer(lazyInstrumentationLoader);
        }
    }
}
