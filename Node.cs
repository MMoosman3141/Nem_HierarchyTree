using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("xUnit.Nem_HierarchyTree")]

namespace Nem_HierarchyTree;

/// <summary>
/// Represents a node in a bit tree structure, holding a name, a numeric value, and references to parent and child nodes.
/// </summary>
public class Node(string name) {
  private readonly object _lock = new();
  private readonly object _propogateLock = new();
  private readonly object _bitFlagLock = new();

  private BigInteger _bitFlag;
  internal List<Node> _children = [];

  [JsonIgnore]
  internal bool IsFalseParent { get; set; } = false;

  /// <summary>
  /// Gets or sets the unique identifier for this node.
  /// </summary>
  [JsonPropertyName("id")]
  public Guid Id { get; set; } = Guid.NewGuid();

  /// <summary>
  /// Gets or sets the name of this node.
  /// </summary>
  [JsonPropertyName("name")]
  public string Name { get; set; } = name;

  [JsonIgnore]
  internal BigInteger BitFlag {
    get {
      lock (_bitFlagLock) {
        return _bitFlag;
      }
    }
    set {
      lock (_bitFlagLock) {
        _bitFlag = value;
        CheckValue |= value;
        PropogateAddToParents(value);
      }
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
  public Node ParentNode { get; set; } = null;

  /// <summary>
  /// Gets or sets the child nodes of this node.
  /// </summary>
  [JsonIgnore]
  public List<Node> Children {
    get {
      lock (_lock) {
        List<Node> children = [];
        children.AddRange(_children);
        return children;
      }
    }
  }

  /// <summary>
  /// Gets the number of child nodes contained in this node.
  /// </summary>
  public int ChildCount {
    get {
      lock (_lock) {
        return _children?.Count ?? 0;
      }
    }
  }

  internal bool AddChild(Node child) {
    lock (_lock) {
      if (_children.Contains(child)) {
        return false;
      }
      child.ParentId = Id;
      child.ParentNode = this;
      _children.Add(child);
      CheckValue |= child.CheckValue;
      PropogateAddToParents(child.CheckValue);

      return true;
    }
  }

  internal Node RemoveChild(Node child) {
    lock (_lock) {
      if (_children.Remove(child)) {
        CheckValue &= ~child.BitFlag;
        PropogateRemoveToParents(child.BitFlag);

        return child;
      }
      return null;
    }
  }

  /// <summary>
  /// Determines whether this node contains the specified node, based on the bitwise check value.
  /// </summary>
  /// <param name="other">The node to check for containment.</param>
  /// <returns>True if this node contains the specified node; otherwise, false.</returns>
  public bool Contains(Node other) {
    return (CheckValue & other.BitFlag) == other.BitFlag;
  }

  /// <summary>
  /// Returns the name of this node.
  /// </summary>
  public override string ToString() {
    return Name;
  }

  /// <summary>
  /// Determines whether two <see cref="Node"/> instances are equal.
  /// </summary>
  /// <param name="left">The first node to compare.</param>
  /// <param name="right">The second node to compare.</param>
  /// <returns>True if the nodes are equal; otherwise, false.</returns>
  public static bool operator ==(Node left, Node right) {
    if (left is null && right is null) {
      return true;
    }
    if (left is null || right is null) {
      return false;
    }
    return left.Equals(right);
  }

  /// <summary>
  /// Determines whether two <see cref="Node"/> instances are not equal.
  /// </summary>
  /// <param name="left">The first node to compare.</param>
  /// <param name="right">The second node to compare.</param>
  /// <returns>True if the nodes are not equal; otherwise, false.</returns>
  public static bool operator !=(Node left, Node right) {
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

    if (obj is not Node) {
      return false;
    }
    Node other = (Node)obj;

    if (Id != other.Id) {
      return false;
    }

    if (Name != other.Name) {
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
    return HashCode.Combine(Id, Name, ParentId);
  }

  private void PropogateAddToParents(BigInteger value) {
    lock (_propogateLock) {
      Node current = ParentNode;
      while (current is not null) {
        current.CheckValue |= value;
        current = current.ParentNode;
      }
    }
  }
  private void PropogateRemoveToParents(BigInteger value) {
    lock (_propogateLock) {
      Node current = ParentNode;
      while (current is not null) {
        current.CheckValue &= ~value;
        current = current.ParentNode;
      }
    }
  }

}
