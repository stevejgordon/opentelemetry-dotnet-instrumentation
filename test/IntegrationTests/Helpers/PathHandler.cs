// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

#if NET8_0_OR_GREATER
using Microsoft.AspNetCore.Http;

namespace IntegrationTests.Helpers;

public class PathHandler
{
    public PathHandler(RequestDelegate @delegate, string path)
    {
        Delegate = @delegate;
        Path = path;
    }

    public RequestDelegate Delegate { get; }

    public string Path { get; }
}
#endif
