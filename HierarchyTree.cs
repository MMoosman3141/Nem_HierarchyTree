using System.Collections;
using System.Numerics;
using System.Text.Json;

namespace Nem_HierarchyTree;

/// <summary>
/// Represents a hierarchical tree structure containing nodes.
/// </summary>
public sealed class HierarchyTree<T> : IEnumerable<Node<T>>, IDisposable {
  readonly List<Node<T>> _roots = [];

  /// <summary>
  /// The maximum number of nodes allowed in the tree.
  /// </summary>
  public int MaxNodes { get; set; } = 2_000;

  private BigInteger _bitFlags = 0;
  private readonly Dictionary<T, Node<T>> _contentsToNode = [];

  /// <summary>
  /// Gets the number of nodes in the tree.
  /// </summary>
  public int Count {
    get => FlatTree.Count;
  }

  /// <summary>
  /// Gets or sets the list of root nodes in the tree.
  /// </summary>
  public IReadOnlyList<Node<T>> Roots => _roots;

  /// <summary>
  /// Gets a flat dictionary of all nodes in the tree, keyed by their unique identifier.
  /// </summary>
  internal Dictionary<Guid, Node<T>> FlatTree { get; private set; } = [];

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
    get {
      ArgumentNullException.ThrowIfNull(contents, nameof(contents));
      return GetNode(contents);
    }
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
      throw new InvalidOperationException("Failed to add node to the tree.");
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
    addedNode = null;

    if (node.Contents is null || node.Id == Guid.Empty) {
      return false;
    }

    BigInteger bitFlag = GetUnsetBit();
    if (bitFlag == 0) {
      return false;
    }

    if (!_contentsToNode.TryAdd(node.Contents, node)) {
      return false;
    }

    if (!FlatTree.TryAdd(node.Id, node)) {
      if (FlatTree[node.Id].IsFalseParent) {
        UpdateFalseParent(node);
        addedNode = node;
        return true;
      }
      addedNode = null;
      return false;
    }

    _bitFlags |= bitFlag;
    node.BitFlag = bitFlag;

    try {
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
    } catch {
      // Rollback only if AddFalseParent fails
      FlatTree.Remove(node.Id);
      _contentsToNode.Remove(node.Contents);
      _bitFlags &= ~bitFlag;
      RecycleBitIndex(bitFlag);

      return false;
    }
  }

  /// <summary>
  /// Removes a node from the tree. If the node has children, they are also removed recursively.
  /// </summary>
  /// <param name="nodeToRemove">The node to remove from the tree.</param>
  /// <returns>A list of all nodes that were removed (includes the specified node and all descendants).</returns>
  /// <exception cref="InvalidOperationException">Thrown when the removal operation fails.</exception>
  public List<Node<T>> Remove(Node<T> nodeToRemove) {
    if (!TryRemove(nodeToRemove, out List<Node<T>> removedNodes)) {
      throw new InvalidOperationException("Failed to remove node from the tree.");
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

    try {
      while (nodesToRemove.Count > 0) {
        Node<T> current = nodesToRemove.Pop();

        if (current.ChildCount > 0) {
          nodesToRemove.Push(current);
          foreach (Node<T> child in current.Children) {
            nodesToRemove.Push(child);
          }
          continue;
        }

        if (!current.IsFalseParent) {
          if (current.ParentId == Guid.Empty) {
            if (!_roots.Remove(current)) {
              throw new InvalidOperationException("Failed to remove root node from the tree.");
            }
          } else {
            if (!current.ParentNode.RemoveChild(current, out _)) {
              throw new InvalidOperationException("Failed to remove child node from its parent.");
            }
          }
          _contentsToNode.Remove(current.Contents);
        }

        if (!FlatTree.Remove(current.Id)) {
          throw new InvalidOperationException("Failed to remove node from the tree.");
        }

        _bitFlags &= ~current.BitFlag;
        RecycleBitIndex(current.BitFlag);
        removed.Add(current);
      }

      removedNodes = removed;
      return true;
    } catch {
      for (int i = removed.Count - 1; i >= 0; i--) {
        TryAdd(removed[i], out _);
      }

      removedNodes = [];
      return false;
    }
  }


  /// <summary>
  /// Cleans the tree by removing any false parents that were never filled in and any orphaned nodes.
  /// </summary>
  public void CleanTree() {
    List<Node<T>> toRemove =
      [.. (from node in FlatTree.Values
      where node.IsFalseParent || (node.ParentId != Guid.Empty && !FlatTree.ContainsKey(node.ParentId))
      select node)];

    foreach (Node<T> node in toRemove) {
      TryRemove(node, out _);
    }
  }

  /// <summary>
  /// Removes all nodes from the tree, resetting its state to empty.
  /// </summary>
  public void Clear() {
    _roots.Clear();
    FlatTree.Clear();
    _contentsToNode.Clear();
    _availableBitIndices.Clear();
    _bitFlags = 0;
    _nextBitIndex = 0;
  }

  /// <summary>
  /// Determines whether the tree contains the specified node instance.
  /// </summary>
  /// <param name="node">The node to locate in the tree.</param>
  /// <returns>True if the node exists in the tree; otherwise, false.</returns>
  public bool Contains(Node<T> node) {
    ArgumentNullException.ThrowIfNull(node, nameof(node));

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
    return _contentsToNode.ContainsKey(contents);
  }

  /// <summary>
  /// Retrieves a node from the tree by its unique identifier.
  /// </summary>
  /// <param name="id">The unique identifier of the node to retrieve.</param>
  /// <returns>The node with the specified identifier, or null if not found.</returns>
  public Node<T> GetNode(Guid id) {
    return FlatTree.TryGetValue(id, out Node<T> node) ? node : null;
  }

  /// <summary>
  /// Retrieves a node from the tree by its contents.
  /// </summary>
  /// <param name="contents">The contents of the node to retrieve.</param>
  /// <returns>The node with the specified contents, or null if not found.</returns>
  public Node<T> GetNode(T contents) {
    return _contentsToNode.TryGetValue(contents, out Node<T> node) ? node : null;
  }

  private readonly Queue<int> _availableBitIndices = new();
  private int _nextBitIndex = 0;

  private BigInteger GetUnsetBit() {
    int index;

    if (_availableBitIndices.Count > 0) {
      index = _availableBitIndices.Dequeue();
    } else {
      if (_nextBitIndex >= MaxNodes) {
        return 0; // Tree is full
      }
      index = _nextBitIndex++;
    }

    return BigInteger.One << index;
  }

  private void RecycleBitIndex(BigInteger bitFlag) {
    if (bitFlag == 0) return;

    int index = (int)BigInteger.Log(bitFlag, 2);
    _availableBitIndices.Enqueue(index);
  }

  private void UpdateFalseParent(Node<T> node) {
    // Do not change the bit flag as the correct value was assigned when the false parent
    // was originally added.
    Node<T> falseParent = FlatTree[node.Id];
    _contentsToNode[node.Contents] = falseParent;
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
  /// Releases all resources used by the <see cref="HierarchyTree{T}"/> instance and resets its state.
  /// </summary>
  public void Dispose() {
    Clear();
  }

  /// <summary>
  /// Serializes a <see cref="HierarchyTree{T}"/> instance to a JSON string.
  /// </summary>
  /// <param name="tree">The <see cref="HierarchyTree{T}"/> instance to serialize.</param>
  /// <param name="serializerOptions">The serializer options to use during serialization.</param>
  /// <returns>A JSON string representing the serialized tree.</returns>
  public static string SerializeJson(HierarchyTree<T> tree, JsonSerializerOptions serializerOptions = default) {
    serializerOptions ??= new JsonSerializerOptions {
      WriteIndented = true
    };

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
    serializerOptions ??= new JsonSerializerOptions {
      WriteIndented = true
    };

    serializerOptions.Converters.Add(new HierarchyTreeJsonConverter<T>());
    return JsonSerializer.Deserialize<HierarchyTree<T>>(json, serializerOptions);
  }
}
