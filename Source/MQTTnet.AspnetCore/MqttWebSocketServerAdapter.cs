﻿using Microsoft.AspNetCore.Http;
using MQTTnet.Adapter;
using MQTTnet.Formatter;
using MQTTnet.Implementations;
using MQTTnet.Server;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using MQTTnet.Diagnostics;

namespace MQTTnet.AspNetCore
{
    public sealed class MqttWebSocketServerAdapter : IMqttServerAdapter
    {
        IMqttNetLogger _logger = new MqttNetNullLogger();
        
        public Func<IMqttChannelAdapter, Task> ClientHandler { get; set; }

        public Task StartAsync(MqttServerOptions options, IMqttNetLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            return Task.CompletedTask;
        }

        public async Task RunWebSocketConnectionAsync(WebSocket webSocket, HttpContext httpContext)
        {
            if (webSocket == null) throw new ArgumentNullException(nameof(webSocket));

            var endpoint = $"{httpContext.Connection.RemoteIpAddress}:{httpContext.Connection.RemotePort}";

            var clientCertificate = await httpContext.Connection.GetClientCertificateAsync().ConfigureAwait(false);
            try
            {
                var isSecureConnection = clientCertificate != null;

                var clientHandler = ClientHandler;
                if (clientHandler != null)
                {
                    var writer = new SpanBasedMqttPacketWriter();
                    var formatter = new MqttPacketFormatterAdapter(writer);
                    var channel = new MqttWebSocketChannel(webSocket, endpoint, isSecureConnection, clientCertificate);

                    using (var channelAdapter = new MqttChannelAdapter(channel, formatter, null, _logger))
                    {
                        await clientHandler(channelAdapter).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                clientCertificate?.Dispose();
            }
        }

        public void Dispose()
        {
        }
    }
}