// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MQTTnet.AspNetCore
{
    public static class EndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Specify the matching path for mqtt-over-websocket
        /// </summary>
        /// <param name="endpoints"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static ConnectionEndpointRouteBuilder MapMqtt(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string pattern)
        {
            return endpoints.MapMqtt(pattern, null);
        }

        /// <summary>
        /// Specify the matching path for mqtt-over-websocket
        /// </summary>
        /// <param name="endpoints"></param>
        /// <param name="pattern"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static ConnectionEndpointRouteBuilder MapMqtt(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string pattern, Action<HttpConnectionDispatcherOptions>? configureOptions)
        {
            // check services.AddMqttServer()
            endpoints.ServiceProvider.GetRequiredService<MqttServer>();

            endpoints.ServiceProvider.GetRequiredService<MqttConnectionHandler>().MapFlag = true;
            return endpoints.MapConnectionHandler<MqttConnectionHandler>(pattern, ConfigureOptions);


            void ConfigureOptions(HttpConnectionDispatcherOptions options)
            {
                options.Transports = HttpTransportType.WebSockets;
                options.WebSockets.SubProtocolSelector = SelectSubProtocol;
                configureOptions?.Invoke(options);
            }

            static string SelectSubProtocol(IList<string> requestedSubProtocolValues)
            {
                // Order the protocols to also match "mqtt", "mqttv-3.1", "mqttv-3.11" etc.
                return requestedSubProtocolValues.OrderByDescending(p => p.Length).FirstOrDefault(p => p.ToLower().StartsWith("mqtt"))!;
            }
        }
    }
}

