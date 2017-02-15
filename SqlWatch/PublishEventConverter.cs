#region

using System;
using Microsoft.SqlServer.XEvent;
using Microsoft.SqlServer.XEvent.Linq;
using Newtonsoft.Json;

#endregion

namespace SqlWatch
{
    class PublishEventConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var x = value as PublishedEvent;

            writer.WriteStartObject();

            writer.WritePropertyName("name");
            writer.WriteValue(x.Name);
            writer.WritePropertyName("uuid");
            writer.WriteValue(x.UUID);
            writer.WritePropertyName("timestamp");
            writer.WriteValue(x.Timestamp);

            writer.WritePropertyName("fields");
            writer.WriteStartArray();
            foreach (PublishedEventField fld in x.Fields)
            {
                WriteProperty(writer, fld.Name, fld.Value, fld.Type);
            }
            writer.WriteEndArray();

            writer.WritePropertyName("actions");
            writer.WriteStartArray();
            foreach (PublishedAction act in x.Actions)
            {
                WriteProperty(writer, act.Name, act.Value, act.Type);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        private static void WriteProperty(JsonWriter writer, string name, object obj, Type type)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(name);
            if (type == typeof (MapValue))
            {
                writer.WriteValue(obj.ToString());
            }
            else if (type == typeof (XMLData))
            {
                var tmp = obj as XMLData;
                writer.WriteValue((tmp == null) ? string.Empty : tmp.RawString);
            }
            else if (type == typeof (CallStack))
            {
                writer.WriteValue(string.Empty);
            }
            else if (type == typeof (ActivityId))
            {
                var tmp = obj as ActivityId;
                writer.WriteValue(tmp.Id);
                writer.WritePropertyName(name+".seq");
                writer.WriteValue(tmp.Sequence);
            }
            else
            {
                writer.WriteValue(obj);
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PublishedEvent);
        }
    }

    public class MapValueConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var x = value as MapValue;
            if (x != null) writer.WriteValue(x.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MapValue);
        }
    }

    public class CallStackConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // var x = value as CallStack;
            // if (x != null) 
            writer.WriteValue(string.Empty);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(CallStack);
        }
    }

    public class XmlDataConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var x = value as XMLData;
            if (x != null) writer.WriteValue(x.RawString);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(XMLData);
        }
    }
}
