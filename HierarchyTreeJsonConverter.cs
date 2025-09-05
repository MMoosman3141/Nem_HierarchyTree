using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nem_HierarchyTree;

/// <summary>
/// Provides JSON serialization and deserialization for <see cref="HierarchyTree"/> objects.
/// </summary>
public class HierarchyTreeJsonConverter : JsonConverter<HierarchyTree> {
  /// <summary>
  /// Reads and converts the JSON to a <see cref="HierarchyTree"/> object.
  /// </summary>
  /// <param name="reader">The reader.</param>
  /// <param name="typeToConvert">The type to convert.</param>
  /// <param name="options">Serializer options.</param>
  /// <returns>The deserialized <see cref="HierarchyTree"/>.</returns>
  public override HierarchyTree Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
    List<Node> nodes = JsonSerializer.Deserialize<List<Node>>(ref reader, options);

    HierarchyTree tree = new();
    foreach (Node node in nodes) {
      tree.Add(node);
    }

    tree.CleanTree();

    return tree;
  }

  /// <summary>
  /// Writes a <see cref="HierarchyTree"/> object as JSON.
  /// </summary>
  /// <param name="writer">The writer.</param>
  /// <param name="value">The tree value.</param>
  /// <param name="options">Serializer options.</param>
  public override void Write(Utf8JsonWriter writer, HierarchyTree value, JsonSerializerOptions options) {
    JsonSerializer.Serialize(writer, value.FlatTree.Values.ToList(), options);
  }
}
