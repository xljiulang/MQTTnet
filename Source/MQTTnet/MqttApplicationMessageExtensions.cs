// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace MQTTnet;

public static class MqttApplicationMessageExtensions
{
    public static string ConvertPayloadToString(this MqttApplicationMessage applicationMessage)
    {
        ArgumentNullException.ThrowIfNull(applicationMessage);

        if (applicationMessage.Payload.Length == 0)
        {
            return null;
        }

        return Encoding.UTF8.GetString(applicationMessage.Payload);
    }

    public static TValue ConvertPayloadToJson<TValue>(this MqttApplicationMessage applicationMessage, JsonTypeInfo<TValue> jsonTypeInfo)
    {
        ArgumentNullException.ThrowIfNull(applicationMessage);

        var jsonOptions = jsonTypeInfo.Options;
        var readerOptions = new JsonReaderOptions
        {
            MaxDepth = jsonOptions.MaxDepth,
            AllowTrailingCommas = jsonOptions.AllowTrailingCommas,
            CommentHandling = jsonOptions.ReadCommentHandling
        };
        var jsonReader = new Utf8JsonReader(applicationMessage.Payload, readerOptions);
        return JsonSerializer.Deserialize(ref jsonReader, jsonTypeInfo);
    }

    public static TValue ConvertPayloadToJson<TValue>(this MqttApplicationMessage applicationMessage, JsonSerializerOptions jsonSerializerOptions = null)
    {
        ArgumentNullException.ThrowIfNull(applicationMessage);

        var readerOptions = new JsonReaderOptions
        {
            MaxDepth = jsonSerializerOptions.MaxDepth,
            AllowTrailingCommas = jsonSerializerOptions.AllowTrailingCommas,
            CommentHandling = jsonSerializerOptions.ReadCommentHandling
        };
        var jsonReader = new Utf8JsonReader(applicationMessage.Payload, readerOptions);
        return JsonSerializer.Deserialize<TValue>(ref jsonReader, jsonSerializerOptions);
    }
}