﻿using System;
using Newtonsoft.Json;

namespace NauraaBot.Core.Utils.JSON;

public class NumericBoolConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is not null)
        {
            writer.WriteValue(((bool)value) ? 1 : 0);
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return reader?.Value?.ToString() == "1";
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(bool) || objectType == typeof(bool?);
    }
}