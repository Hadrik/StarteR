using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using StarteR.Models.Steps;

namespace StarteR.StepManagement;

public class StepModelConverter : JsonConverter<StepModelBase>
{
    private const string TypeDiscriminatorProperty = "$type";

    public override StepModelBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);

        if (!doc.RootElement.TryGetProperty(TypeDiscriminatorProperty, out var discriminatorElement))
            throw new JsonException($"Missing '{TypeDiscriminatorProperty}' property for polymorphic deserialization");

        var discriminator = discriminatorElement.GetString()
            ?? throw new JsonException($"'{TypeDiscriminatorProperty}' property cannot be null");

        var stepInfo = StepRegistry.GetInfoByDiscriminator(discriminator)
            ?? throw new JsonException($"Unknown step type discriminator: {discriminator}");

        var json = doc.RootElement.GetRawText();

        // Create options without this converter to avoid infinite recursion
        var innerOptions = new JsonSerializerOptions(options);
        innerOptions.Converters.Remove(this);

        return (StepModelBase?)JsonSerializer.Deserialize(json, stepInfo.ModelType, innerOptions);
    }

    public override void Write(Utf8JsonWriter writer, StepModelBase value, JsonSerializerOptions options)
    {
        var stepInfo = StepRegistry.GetInfo(value.GetType());

        // Create options without this converter to avoid infinite recursion
        var innerOptions = new JsonSerializerOptions(options);
        innerOptions.Converters.Remove(this);

        writer.WriteStartObject();
        writer.WriteString(TypeDiscriminatorProperty, stepInfo.TypeDiscriminator);

        // Serialize the concrete type and merge properties
        using var doc = JsonSerializer.SerializeToDocument(value, value.GetType(), innerOptions);
        foreach (var property in doc.RootElement.EnumerateObject())
        {
            if (property.Name != TypeDiscriminatorProperty)
            {
                property.WriteTo(writer);
            }
        }

        writer.WriteEndObject();
    }
}