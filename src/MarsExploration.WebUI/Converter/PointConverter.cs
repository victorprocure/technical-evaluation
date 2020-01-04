using System;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace MarsExploration.WebUI.Converter
{
    public class PointConverter : CustomCreationConverter<Point>
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override Point Create(Type objectType)
        {
            return new Point();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            return new Point(jObject["x"].Value<int>(), jObject["y"].Value<int>());
        }
    }
}
