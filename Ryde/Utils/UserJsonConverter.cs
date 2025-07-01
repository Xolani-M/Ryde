using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ryde;

namespace Utils
{
    public class UserJsonConverter : JsonConverter<User>
    {
        public override User Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (var jsonDoc = JsonDocument.ParseValue(ref reader))
            {
                var root = jsonDoc.RootElement;
                var userType = root.GetProperty("UserType").GetString();
                switch (userType)
                {
                    case "Passenger":
                        return JsonSerializer.Deserialize<Passenger>(root.GetRawText(), options);
                    case "Driver":
                        return JsonSerializer.Deserialize<Driver>(root.GetRawText(), options);
                    default:
                        throw new NotSupportedException($"Unknown user type: {userType}");
                }
            }
        }

        public override void Write(Utf8JsonWriter writer, User value, JsonSerializerOptions options)
        {
            if (value is Passenger)
            {
                writer.WriteStartObject();
                foreach (var prop in value.GetType().GetProperties())
                {
                    writer.WritePropertyName(prop.Name);
                    JsonSerializer.Serialize(writer, prop.GetValue(value), options);
                }
                writer.WriteString("UserType", "Passenger");
                writer.WriteEndObject();
            }
            else if (value is Driver)
            {
                writer.WriteStartObject();
                foreach (var prop in value.GetType().GetProperties())
                {
                    writer.WritePropertyName(prop.Name);
                    JsonSerializer.Serialize(writer, prop.GetValue(value), options);
                }
                writer.WriteString("UserType", "Driver");
                writer.WriteEndObject();
            }
            else
            {
                throw new NotSupportedException($"Unknown user type: {value.GetType().Name}");
            }
        }
    }
}
