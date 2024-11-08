// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MQTTnet.Diagnostics.Logger;
using System.Threading.Tasks;

namespace MQTTnet.Adapter;

public interface IMqttClientAdapterFactory
{
    ValueTask<IMqttChannelAdapter> CreateClientAdapterAsync(MqttClientOptions options, MqttPacketInspector packetInspector, IMqttNetLogger logger);
}