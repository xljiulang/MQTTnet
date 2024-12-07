// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using MQTTnet.Formatter;
using System.Threading;
using System.Threading.Tasks;

namespace MQTTnet.AspNetCore;

sealed class MqttServerChannelAdapter : MqttChannel, IAspNetCoreMqttChannelAdapter
{
    public HttpContext? HttpContext { get; }
    public IFeatureCollection? Features { get; }

    public MqttServerChannelAdapter(MqttPacketFormatterAdapter packetFormatterAdapter, ConnectionContext connection, HttpContext? httpContext)
        : base(packetFormatterAdapter, connection, httpContext, packetInspector: null)
    {
        HttpContext = httpContext;
        Features = connection.Features;

        SetAllowPacketFragmentation(connection, httpContext);
    }

    private void SetAllowPacketFragmentation(ConnectionContext connection, HttpContext? httpContext)
    {
        // When connection is from MapMqtt(),
        // the PacketFragmentationFeature instance is copied from kestrel's ConnectionContext.Features to HttpContext.Features,
        // but no longer from HttpContext.Features to connection.Features.     
        var packetFragmentationFeature = httpContext == null
            ? connection.Features.Get<PacketFragmentationFeature>()
            : httpContext.Features.Get<PacketFragmentationFeature>();

        if (packetFragmentationFeature == null)
        {
            var value = PacketFragmentationFeature.CanAllowPacketFragmentation(this, null);
            SetAllowPacketFragmentation(value);
        }
        else
        {
            var value = packetFragmentationFeature.AllowPacketFragmentationSelector(this);
            SetAllowPacketFragmentation(value);
        }
    }

    /// <summary>
    /// This method will never be called
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task ConnectAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task DisconnectAsync(CancellationToken cancellationToken)
    {
        return base.DisconnectAsync();
    }
}