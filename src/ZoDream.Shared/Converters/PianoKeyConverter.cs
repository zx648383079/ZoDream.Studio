using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Converters
{
    public class PianoKeyConverter : JsonConverter<PianoKey>
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString()!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return PianoKey.Create(value);
        }

        public override PianoKey? ReadJson(JsonReader reader, Type objectType, PianoKey? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is null)
            {
                return null;
            }
            return PianoKey.Create(reader.Value);
        }

        public override void WriteJson(JsonWriter writer, PianoKey? value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString());
        }
    }
}
