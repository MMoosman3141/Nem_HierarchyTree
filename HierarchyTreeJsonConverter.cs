using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nem_HierarchyTree;

/// <summary>
/// Provides JSON serialization and deserialization for <see cref="HierarchyTree{T}"/> objects.
/// </summary>
public class HierarchyTreeJsonConverter<T> : JsonConverter<HierarchyTree<T>> {
  /// <summary>
  /// Reads and converts the JSON to a <see cref="HierarchyTree{T}"/> object.
  /// </summary>
  /// <param name="reader">The reader.</param>
  /// <param name="typeToConvert">The type to convert.</param>
  /// <param name="options">Serializer options.</param>
  /// <returns>The deserialized <see cref="HierarchyTree{T}"/>.</returns>
  public override HierarchyTree<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
    List<Node<T>> nodes = JsonSerializer.Deserialize<List<Node<T>>>(ref reader, options);

    HierarchyTree<T> tree = [];
    foreach (Node<T> node in nodes) {
      tree.Add(node);
    }

    tree.CleanTree();

    return tree;
  }

  /// <summary>
  /// Writes a <see cref="HierarchyTree{T}"/> object as JSON.
  /// </summary>
  /// <param name="writer">The writer.</param>
  /// <param name="value">The tree value.</param>
  /// <param name="options">Serializer options.</param>
  public override void Write(Utf8JsonWriter writer, HierarchyTree<T> value, JsonSerializerOptions options) {
    JsonSerializer.Serialize(writer, value.FlatTree.Values.ToList(), options);
  }
}
