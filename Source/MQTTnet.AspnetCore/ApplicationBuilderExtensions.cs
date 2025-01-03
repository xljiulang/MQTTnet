// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Server;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MQTTnet.AspNetCore;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Get and use <see cref="MqttServer"/>
    /// </summary>
    /// <remarks>Also, you can inject <see cref="MqttServer"/> into your service</remarks>
    /// <param name="app"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseMqttServer(this IApplicationBuilder app, Action<MqttServer> configure)
    {
        var server = app.ApplicationServices.GetRequiredService<MqttServer>();
        configure(server);
        return app;
    }

    /// <summary>
    /// Active MqttServer's wrapper service
    /// </summary>
    /// <typeparam name="TMQttServerWrapper"></typeparam>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseMqttServer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMQttServerWrapper>(this IApplicationBuilder app)
    {
        ActivatorUtilities.GetServiceOrCreateInstance<TMQttServerWrapper>(app.ApplicationServices);
        return app;
    }
}