using System.Collections;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nem_HierarchyTree;

/// <summary>
/// Represents a hierarchical tree structure containing nodes.
/// </summary>
public sealed class HierarchyTree<T> : IEnumerator<Node<T>>, IEnumerable<Node<T>> {
  readonly List<Node<T>> _roots = [];

  /// <summary>
  /// The maximum number of nodes allowed in the tree.
  /// </summary>
  public int MaxNodes { get; set; } = 2_000;

  private BigInteger _bitFlags = 0;
  private readonly HashSet<T> _nodeContents = [];

  /// <summary>
  /// Gets the number of nodes in the tree.
  /// </summary>
  public int Count {
    get => FlatTree.Count;
  }

  /// <summary>
  /// Gets or sets the list of root nodes in the tree.
  /// </summary>
  public IReadOnlyList<Node<T>> Roots {
    get {
      IReadOnlyList<Node<T>> readonlyList = [.. _roots];
      return readonlyList;
    }
  }

  /// <summary>
  /// Gets a flat dictionary of all nodes in the tree, keyed by their unique identifier.
  /// </summary>
  internal Dictionary<Guid, Node<T>> FlatTree { get; private set; } = [];

  private Exception _exceptionValue = null;

  /// <summary>
  /// Retrieves a node from the tree by its unique identifier.
  /// </summary>
  /// <param name="id">The unique identifier of the node to retrieve.</param>
  /// <returns>The node with the specified identifier, or null if not found.</returns>
  public Node<T> this[Guid id] {
    get => GetNode(id);
  }

  /// <summary>
  /// Retrieves a node from the tree by its name.
  /// </summary>
  /// <param name="contents">The contents of the node to retrieve.</param>
  /// <returns>The node with the specified name, or null if not found.</returns>
  public Node<T> this[T contents] {
    get => GetNode(contents);
  }

  /// <summary>
  /// Adds a node to the tree. Throws an exception if the node cannot be added.
  /// </summary>
  /// <param name="node">The node to add to the tree.</param>
  /// <returns>The added node.</returns>
  /// <exception cref="InvalidOperationException">
  /// Thrown when the node cannot be added due to invalid data or tree constraints.
  /// </exception>
  public Node<T> Add(Node<T> node) {
    if (!TryAdd(node, out Node<T> addedNode)) {
      if (_exceptionValue != null) {
        throw _exceptionValue;
      } else {
        throw new InvalidOperationException("Failed to add node to the tree for an unknown reason.");
      }
    }
    return addedNode;
  }

  /// <summary>
  /// Attempts to add a node to the tree. Returns true if the node was added successfully; otherwise, false.
  /// </summary>
  /// <param name="node">The node to add to the tree.</param>
  /// <returns>True if the node was added successfully; otherwise, false.</returns>
  public bool TryAdd(Node<T> node) {
    return TryAdd(node, out Node<T> _);
  }

  /// <summary>
  /// Attempts to add a node to the tree and outputs the added node if successful.
  /// Returns true if the node was added successfully; otherwise, false.
  /// </summary>
  /// <param name="node">The node to add to the tree.</param>
  /// <param name="addedNode">The node that was added, or null if the addition failed.</param>
  /// <returns>True if the node was added successfully; otherwise, false.</returns>
  public bool TryAdd(Node<T> node, out Node<T> addedNode) {
    BigInteger bitFlag = 0;
    bool nameAdded = true;

    try {
      if (node.Contents is null) {
        throw new InvalidOperationException("Node contents cannot be null.");
      }

      if (node.Id == Guid.Empty) {
        throw new InvalidOperationException("Node ID cannot be an empty GUID.");
      }

      bitFlag = GetUnsetBit();
      if (bitFlag == 0) {
        throw new InvalidOperationException("The tree is full. No more nodes can be added.");
      }

      if (!_nodeContents.Add(node.Contents)) {
        nameAdded = false;
        addedNode = null;
        throw new InvalidOperationException($"A node with identical '{node}' already exists in the tree. Node names must be unique.");
      }

      if (!FlatTree.TryAdd(node.Id, node)) {
        if (FlatTree[node.Id].IsFalseParent) {
          // If the node was added as a false parent, update it.
          UpdateFalseParent(node);

          addedNode = node;
          return true;
        } else {
          if (nameAdded) {
            _nodeContents.Remove(node.Contents);
          }
          addedNode = null;
          return false;
        }
      }

      _bitFlags |= bitFlag;
      node.BitFlag = bitFlag;

      if (node.ParentId != Guid.Empty) {
        if (!FlatTree.TryGetValue(node.ParentId, out Node<T> parent)) {
          parent = AddFalseParent(node);
        }
        parent.AddChild(node);
      } else {
        _roots.Add(node);
      }
      addedNode = node;
      return true;
    } catch (Exception e) {
      _roots.Remove(node);
      FlatTree.Remove(node.Id);
      if (nameAdded) {
        _nodeContents.Remove(node.Contents);
      }
      if (bitFlag != 0) {
        _bitFlags &= ~bitFlag;
      }

      _exceptionValue = e;

      addedNode = null;
      return false;
    }
  }


  /// <summary>
  /// Removes a node from the tree. If the node has children, they are also removed iteratively.
  /// Returns true if the node and its children were removed successfully; otherwise, false.
  /// </summary>
  /// <param name="nodeToRemove">The node to remove from the tree.</param>
  /// <returns>True if the node was removed successfully; otherwise, false.</returns>
  public List<Node<T>> Remove(Node<T> nodeToRemove) {
    if (!TryRemove(nodeToRemove, out List<Node<T>> removedNodes)) {
      if (_exceptionValue != null) {
        throw _exceptionValue;
      } else {
        throw new InvalidOperationException("Failed to remove node from the tree for an unknown reason.");
      }
    }
    return removedNodes;
  }

  /// <summary>
  /// Attempts to remove a node from the tree. Returns true if the node and its children were removed successfully; otherwise, false.
  /// </summary>
  /// <param name="nodeToRemove">The node to remove from the tree.</param>
  /// <returns>True if the node was removed successfully; otherwise, false.</returns>
  public bool TryRemove(Node<T> nodeToRemove) {
    return TryRemove(nodeToRemove, out List<Node<T>> _);
  }

  /// <summary>
  /// Attempts to remove a node from the tree and outputs the list of removed nodes.
  /// Returns true if the node and its children were removed successfully; otherwise, false.
  /// </summary>
  /// <param name="nodeToRemove">The node to remove from the tree.</param>
  /// <param name="removedNodes">The list of nodes that were removed, or an empty list if the removal failed.</param>
  /// <returns>True if the node and its children were removed successfully; otherwise, false.</returns>
  public bool TryRemove(Node<T> nodeToRemove, out List<Node<T>> removedNodes) {
    List<Node<T>> removed = [];
    Stack<Node<T>> nodesToRemove = [];
    nodesToRemove.Push(nodeToRemove);

    bool removedRoot = false;
    bool removedChild = false;

    try {
      while (nodesToRemove.TryPeek(out Node<T> current)) {
        if (current.ChildCount > 0) {
          foreach (Node<T> child in current.Children) {
            nodesToRemove.Push(child);
          }
          continue;
        }

        nodesToRemove.Pop();

        if (!current.IsFalseParent) {
          if (current.ParentId == Guid.Empty) {
            if (!_roots.Remove(current)) {
              throw new InvalidOperationException("Failed to remove root node from the tree.");
            }
            removedRoot = true;
          } else {
            if (current.ParentNode.RemoveChild(current) is null) {
              throw new InvalidOperationException("Failed to remove child node from its parent.");
            }
            removedChild = true;
          }
        }

        if (!FlatTree.Remove(current.Id)) {
          if (removedRoot) {
            _roots.Add(current);
          }
          if (removedChild) {
            current.ParentNode.AddChild(current);
          }
          throw new InvalidOperationException("Failed to remove node from the tree.");
        }

        _bitFlags &= ~current.BitFlag;
        _nodeContents.Remove(current.Contents);
      }
      removed.Add(nodeToRemove);
      removedNodes = removed;
      return true;
    } catch (Exception e) {
      _exceptionValue = e;
      // Attempt to restore any nodes that were removed before the error occurred.
      foreach (Node<T> node in removed) {
        Add(node);
      }

      removedNodes = [];
      return false;
    }
  }


  /// <summary>
  /// Cleans the tree by removing any false parents that were never filled in and any orphaned nodes.
  /// </summary>
  public void CleanTree() {
    // Clear out any false parents that were never filled in.
    List<Node<T>> falseParents = [.. FlatTree.Values.Where(n => n.IsFalseParent)];
    foreach (Node<T> falseParent in falseParents) {
      Remove(falseParent);
    }

    // Clear out any orphaned nodes
    List<Node<T>> orphanedNodes = [.. FlatTree.Values.Where(n => n.ParentId != Guid.Empty && !FlatTree.ContainsKey(n.ParentId))];
    foreach (Node<T> orphan in orphanedNodes) {
      Remove(orphan);
    }
  }

  /// <summary>
  /// Removes all nodes from the tree, resetting its state to empty.
  /// </summary>
  public void Clear() {
    Dispose();
  }

  /// <summary>
  /// Determines whether the tree contains the specified node instance.
  /// </summary>
  /// <param name="node">The node to locate in the tree.</param>
  /// <returns>True if the node exists in the tree; otherwise, false.</returns>
  public bool Contains(Node<T> node) {
    if (!FlatTree.TryGetValue(node.Id, out Node<T> value)) {
      return false;
    }

    return value.Equals(node);
  }

  /// <summary>
  /// Determines whether the tree contains a node with the specified unique identifier.
  /// </summary>
  /// <param name="id">The unique identifier of the node to locate.</param>
  /// <returns>True if the node exists in the tree; otherwise, false.</returns>
  public bool Contains(Guid id) {
    return FlatTree.ContainsKey(id);
  }

  /// <summary>
  /// Determines whether the tree contains a node with the specified contents.
  /// </summary>
  /// <param name="contents">The contents of the node to locate.</param>
  /// <returns>True if a node with the specified contents exists in the tree; otherwise, false.</returns>
  public bool Contains(T contents) {
    return _nodeContents.Contains(contents);
  }

  /// <summary>
  /// Retrieves a node from the tree by its unique identifier.
  /// </summary>
  /// <param name="id">The unique identifier of the node to retrieve.</param>
  /// <returns>The node with the specified identifier, or null if not found.</returns>
  public Node<T> GetNode(Guid id) {
    if (FlatTree.TryGetValue(id, out Node<T> node)) {
      return node;
    }
    return null;
  }

  /// <summary>
  /// Retrieves a node from the tree by its contents.
  /// </summary>
  /// <param name="contents">The contents of the node to retrieve.</param>
  /// <returns>The node with the specified contents, or null if not found.</returns>
  public Node<T> GetNode(T contents) {
    return FlatTree.Values.FirstOrDefault(n => n.Contents.Equals(contents));
  }

  private BigInteger GetUnsetBit() {
    for (int i = 0; i < MaxNodes; i++) {
      BigInteger bit = BigInteger.One << i;
      if ((_bitFlags & bit) == 0) {
        return bit;
      }
    }
    return 0; // All bits are set
  }

  private void UpdateFalseParent(Node<T> node) {
    // Do not change the bit flag as the correct value was assigned when the false parent
    // was originally added.
    Node<T> falseParent = FlatTree[node.Id];
    falseParent.ParentId = node.ParentId;
    falseParent.Contents = node.Contents;
    falseParent.IsFalseParent = false;

    if (falseParent.ParentId == Guid.Empty) {
      if (!Roots.Contains(falseParent)) {
        _roots.Add(falseParent);
      }
    } else {
      if (!FlatTree.TryGetValue(falseParent.ParentId, out Node<T> parent)) {
        parent = AddFalseParent(falseParent);
      }
      parent.AddChild(falseParent);
    }
  }

  private Node<T> AddFalseParent(Node<T> childNode) {
    BigInteger falseParentBitFlag = GetUnsetBit();
    if (falseParentBitFlag == 0) {
      throw new InvalidOperationException("The tree is full. Error adding parent node.");
    }
    _bitFlags |= falseParentBitFlag;

    Node<T> falseParent = new(default) {
      IsFalseParent = true,
      Id = childNode.ParentId,
      BitFlag = falseParentBitFlag
    };
    FlatTree.Add(falseParent.Id, falseParent);
    return falseParent;
  }

  /// <summary>
  /// Returns an enumerator that iterates through the nodes in the tree using pre-order traversal.
  /// </summary>
  /// <returns>An enumerator for the nodes in the tree.</returns>
  public IEnumerator<Node<T>> GetEnumerator() {
    // Pre-order traversal: children, siblings, then parents
    if (Roots.Count == 0)
      yield break;

    Stack<Node<T>> stack = new();
    // Push roots in reverse order so the first root is visited first
    for (int i = Roots.Count - 1; i >= 0; i--) {
      stack.Push(Roots[i]);
    }

    while (stack.Count > 0) {
      Node<T> node = stack.Pop();
      yield return node;

      // Push children in reverse order so the first child is visited first
      if (node.ChildCount > 0) {
        for (int i = node.ChildCount - 1; i >= 0; i--) {
          stack.Push(node.Children[i]);
        }
      }
    }
  }

  IEnumerator IEnumerable.GetEnumerator() {
    return GetEnumerator();
  }

  /// <summary>
  /// Gets or sets the current node in the tree during enumeration.
  /// </summary>
  public Node<T> Current { get; set; } = null;

  /// <summary>
  /// Advances the enumerator to the next node in the tree using pre-order traversal.
  /// </summary>
  /// <returns>
  /// True if the enumerator was successfully advanced to the next node; false if the end of the tree has been reached.
  /// </returns>
  public bool MoveNext() {
    // If Current is null, start at the first root node (pre-order traversal)
    if (Current == null) {
      if (Roots.Count == 0) {
        return false;
      }
      Current = Roots[0];
      return true;
    }

    // 1. Go to first child if any
    if (Current.ChildCount > 0) {
      Current = Current.Children[0];
      return true;
    }

    // 2. Go to next sibling, or ancestor's next sibling
    Node<T> node = Current;
    while (node != null) {
      Node<T> parent = node.ParentNode;
      List<Node<T>> siblings = (List<Node<T>>)(parent == null ? Roots : parent.Children);
      int idx = siblings.IndexOf(node);
      if (idx >= 0 && idx + 1 < siblings.Count) {
        Current = siblings[idx + 1];
        return true;
      }
      node = parent;
    }

    // 3. No more nodes
    return false;
  }

  /// <summary>
  /// Resets the enumerator to its initial position, which is before the first node in the tree.
  /// </summary>
  public void Reset() {
    Current = null;
  }

  object IEnumerator.Current => Current;

  /// <summary>
  /// Releases all resources used by the <see cref="HierarchyTree{T}"/> instance and resets its state.
  /// </summary>
  public void Dispose() {
    // No unmanaged resources to release, but clear references for GC.
    Reset();
    _roots.Clear();
    FlatTree.Clear();
    _nodeContents.Clear();
    _bitFlags = 0;
    _exceptionValue = null;
    Current = null;
    GC.SuppressFinalize(this);
  }



  /// <summary>
  /// Serializes a <see cref="HierarchyTree{T}"/> instance to a JSON string.
  /// </summary>
  /// <param name="tree">The <see cref="HierarchyTree{T}"/> instance to serialize.</param>
  /// <param name="serializerOptions">The serializer options to use during serialization.</param>
  /// <returns>A JSON string representing the serialized tree.</returns>
  public static string SerializeJson(HierarchyTree<T> tree, JsonSerializerOptions serializerOptions = default) {
    if (serializerOptions is null) {
      serializerOptions = new JsonSerializerOptions {
        WriteIndented = true
      };
    }

    serializerOptions.Converters.Add(new HierarchyTreeJsonConverter<T>());
    return JsonSerializer.Serialize(tree, serializerOptions);
  }

  /// <summary>
  /// Deserializes a JSON string into a <see cref="HierarchyTree{T}"/> instance.
  /// </summary>
  /// <param name="json">The JSON string representing the tree.</param>
  /// <param name="serializerOptions">The serializer options to use during deserialization.</param>
  /// <returns>A <see cref="HierarchyTree{T}"/> instance deserialized from the JSON string.</returns>
  public static HierarchyTree<T> DeserializeJson(string json, JsonSerializerOptions serializerOptions = default) {
    if (serializerOptions is null) {
      serializerOptions = new JsonSerializerOptions {
        WriteIndented = true
      };
    }

    serializerOptions.Converters.Add(new HierarchyTreeJsonConverter<T>());
    return JsonSerializer.Deserialize<HierarchyTree<T>>(json, serializerOptions);
  }
}
