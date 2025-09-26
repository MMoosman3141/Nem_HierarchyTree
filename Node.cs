using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("xUnit.Nem_HierarchyTree")]

namespace Nem_HierarchyTree;

/// <summary>
/// Represents a node in a bit tree structure, holding a name, a numeric value, and references to parent and child nodes.
/// </summary>
public sealed class Node<T>(T contents) {
  private BigInteger _bitFlag;
  private readonly List<Node<T>> _children = [];

  [JsonIgnore]
  internal bool IsFalseParent { get; set; } = false;

  /// <summary>
  /// Gets or sets the unique identifier for this node.
  /// </summary>
  [JsonPropertyName("id")]
  public Guid Id { get; init; } = Guid.NewGuid();

  /// <summary>
  /// Gets or sets the name of this node.
  /// </summary>
  [JsonPropertyName("contents")]
  public T Contents { get; internal set; } = contents;

  [JsonIgnore]
  internal BigInteger BitFlag {
    get => _bitFlag;
    set {
      _bitFlag = value;
      InsertCheckValue(value);
      PropogateAddToParents(value);
    }
  }

  [JsonIgnore]
  internal BigInteger CheckValue { get; set; }

  /// <summary>
  /// Gets or sets the unique identifier of the parent node.
  /// </summary>
  [JsonPropertyName("parentId")]
  public Guid ParentId { get; set; } = Guid.Empty;

  /// <summary>
  /// Gets or sets the parent node of this node.
  /// </summary>
  [JsonIgnore]
  public Node<T> ParentNode { get; internal set; } = null;

  /// <summary>
  /// Gets or sets the child nodes of this node.
  /// </summary>
  [JsonIgnore]
  public IReadOnlyList<Node<T>> Children {
    get {
      IReadOnlyList<Node<T>> readonlyList = [.. _children];
      return readonlyList;
    }
  }

  /// <summary>
  /// Gets the number of child nodes contained in this node.
  /// </summary>
  public int ChildCount {
    get {
      return _children?.Count ?? 0;
    }
  }

  internal bool AddChild(Node<T> child) {


    if (_children.Contains(child)) {
      return false;
    }
    child.ParentId = Id;
    child.ParentNode = this;
    _children.Add(child);

    BigInteger childCheckValue;
    childCheckValue = child.CheckValue;
    InsertCheckValue(childCheckValue);

    PropogateAddToParents(child.CheckValue);

    return true;
  }


  internal Node<T> RemoveChild(Node<T> child) {
    if (_children.Remove(child)) {
      BigInteger childCheckValue;
      childCheckValue = child.CheckValue;
      RemoveCheckValue(childCheckValue);

      PropogateRemoveToParents(child.BitFlag);

      return child;
    }
    return null;
  }

  /// <summary>
  /// Determines whether this node contains the specified node, based on the bitwise check value.
  /// </summary>
  /// <param name="other">The node to check for containment.</param>
  /// <returns>True if this node contains the specified node; otherwise, false.</returns>
  public bool Contains(Node<T> other) {
    BigInteger snapshot;
    snapshot = CheckValue;
    return (snapshot & other.BitFlag) == other.BitFlag;
  }

  /// <summary>
  /// Returns the name of this node.
  /// </summary>
  public override string ToString() {
    return Contents.ToString();
  }

  /// <summary>
  /// Determines whether two <see cref="Node{T}"/> instances are not equal.
  /// </summary>
  /// <param name="left">The first node to compare.</param>
  /// <param name="right">The second node to compare.</param>
  /// <returns>True if the nodes are not equal; otherwise, false.</returns>
  public static bool operator ==(Node<T> left, Node<T> right) {
    if (left is null && right is null) {
      return true;
    }
    if (left is null || right is null) {
      return false;
    }
    return left.Equals(right);
  }

  /// <summary>
  /// Determines whether two <see cref="Node{T}"/> instances are not equal.
  /// </summary>
  /// <param name="left">The first node to compare.</param>
  /// <param name="right">The second node to compare.</param>
  /// <returns>True if the nodes are not equal; otherwise, false.</returns>
  public static bool operator !=(Node<T> left, Node<T> right) {
    return !(left == right);
  }

  /// <summary>
  /// Determines whether the specified object is equal to the current node.
  /// </summary>
  /// <param name="obj">The object to compare with the current node.</param>
  /// <returns>True if the specified object is equal to the current node; otherwise, false.</returns>
  public override bool Equals(object obj) {
    if (obj is null) {
      return false;
    }

    if (ReferenceEquals(this, obj)) {
      return true;
    }

    if (obj is not Node<T>) {
      return false;
    }
    Node<T> other = (Node<T>)obj;

    if (Id != other.Id) {
      return false;
    }

    if (!Contents.Equals(other.Contents)) {
      return false;
    }

    if (ParentId != other.ParentId) {
      return false;
    }

    return true;
  }

  /// <summary>
  /// Returns a hash code for this node.
  /// </summary>
  public override int GetHashCode() {
    return HashCode.Combine(Id, Contents, ParentId);
  }

  private void PropogateAddToParents(BigInteger value) {
    Node<T> current = ParentNode;
    while (current is not null) {
      current.InsertCheckValue(value);
      current = current.ParentNode;
    }
  }
  private void PropogateRemoveToParents(BigInteger value) {
    Node<T> current = ParentNode;
    while (current is not null) {
      current.RemoveCheckValue(value);
      current = current.ParentNode;
    }
  }

  private void InsertCheckValue(BigInteger value) {
    CheckValue |= value;
  }

  private void RemoveCheckValue(BigInteger value) {
    CheckValue &= ~value;
  }

}
