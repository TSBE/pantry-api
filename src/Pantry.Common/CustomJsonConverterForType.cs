using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pantry.Common;

/// <summary>
/// Convert type to string as assembly qualified name.
/// Copied from https://stackoverflow.com/a/66963611.
/// </summary>
public class CustomJsonConverterForType : JsonConverter<Type>
{
    /// <inheritdoc/>
    public override Type Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        /*
         * Caution: Deserialization of type instances like this
         * is not recommended and should be avoided
         * since it can lead to potential security issues.
         * If you really want this supported (for instance if the JSON input is trusted):
         * string assemblyQualifiedName = reader.GetString();
         * return Type.GetType(assemblyQualifiedName);
        */

        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        Type value,
        JsonSerializerOptions options)
    {
        var assemblyQualifiedName = value.AssemblyQualifiedName;

        // Use this with caution, since you are disclosing type information.
        writer.WriteStringValue(assemblyQualifiedName);
    }
}
