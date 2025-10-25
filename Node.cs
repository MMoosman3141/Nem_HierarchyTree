using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("xUnit.Nem_HierarchyTree")]

namespace Nem_HierarchyTree;

/// <summary>
/// Represents a node in a bit tree structure, holding a name, a numeric value, and references to parent and child nodes.
/// </summary>
public sealed class Node<T>(T contents) : IEquatable<Node<T>> where T : notnull  {
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
      PropagateAddToParents(value);
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
  /// Gets the child nodes of this node.
  /// </summary>
  /// <remarks>
  /// Returns a read-only view that reflects the current state of the children collection.
  /// The collection maintains insertion order.
  /// </remarks>
  [JsonIgnore]
  public IReadOnlyList<Node<T>> Children => _children;

  /// <summary>
  /// Gets the number of child nodes contained in this node.
  /// </summary>
  public int ChildCount => _children.Count;

  internal bool AddChild(Node<T> child) {
    ArgumentNullException.ThrowIfNull(child);

    if (Contains(child)) {
      return false;
    }

    if (IsAncestorOf(child)) {
      throw new InvalidOperationException("Cannot add ancestor as child.");
    }

    child.ParentId = Id;
    child.ParentNode = this;
    _children.Add(child);

    InsertCheckValue(child.CheckValue);
    PropagateAddToParents(child.CheckValue);

    return true;
  }

  internal bool RemoveChild(Node<T> child, out Node<T> removed) {
    ArgumentNullException.ThrowIfNull(child);

    if (Contains(child) && _children.Remove(child)) {
      child.ParentNode = null;
      child.ParentId = Guid.Empty;
      RemoveCheckValue(child.CheckValue);
      PropagateRemoveToParents(child.BitFlag);
      removed = child;
      return true;
    }
    removed = null;
    return false;
  }

  /// <summary>
  /// Determines whether this node contains the specified node, based on the bitwise check value.
  /// </summary>
  /// <param name="other">The node to check for containment.</param>
  /// <returns>True if this node contains the specified node; otherwise, false.</returns>
  public bool Contains(Node<T> other) {
    return (CheckValue & other.BitFlag) == other.BitFlag;
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
  /// Determines whether the specified <see cref="Node{T}"/> is equal to the current node.
  /// </summary>
  /// <param name="other">The node to compare with the current node.</param>
  /// <returns>True if the specified node is equal to the current node; otherwise, false.</returns>
  public bool Equals(Node<T> other) {
    if (other is null) {
      return false;
    }

    if (ReferenceEquals(this, other)) {
      return true;
    }

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
  /// Determines whether the specified object is equal to the current node.
  /// </summary>
  /// <param name="obj">The object to compare with the current node.</param>
  /// <returns>True if the specified object is equal to the current node; otherwise, false.</returns>
  public override bool Equals(object obj) {
    return obj is Node<T> other && Equals(other);
  }

  /// <summary>
  /// Returns a hash code for this node.
  /// </summary>
  public override int GetHashCode() {
    return HashCode.Combine(Id, Contents, ParentId);
  }

  private bool IsAncestorOf(Node<T> potentialDescendant) {
    Node<T> current = this;
    while (current != null) {
      if (current == potentialDescendant) {
        return true;
      }
      current = current.ParentNode;
    }
    return false;
  }

  private void PropagateAddToParents(BigInteger value) {
    Node<T> current = ParentNode;
    while (current is not null) {
      current.InsertCheckValue(value);
      current = current.ParentNode;
    }
  }
  private void PropagateRemoveToParents(BigInteger value) {
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
