using System.Numerics;
using System.Text.Json.Serialization;

namespace Nem_HierarchyTree {
  /// <summary>
  /// Represents a hierarchical tree structure containing nodes.
  /// </summary>
  [JsonConverter(typeof(HierarchyTreeJsonConverter))]
  public class HierarchyTree {
    /// <summary>
    /// The maximum number of nodes allowed in the tree.
    /// </summary>
    public int MaxNodes { get; set; } = 2_000;

    private BigInteger _bitFlags = 0;
    private readonly HashSet<string> _nodeNames = [];

    /// <summary>
    /// Gets the number of nodes in the tree.
    /// </summary>
    public int Count {
      get => FlatTree.Count;
    }

    /// <summary>
    /// Gets or sets the list of root nodes in the tree.
    /// </summary>
    public List<Node> Roots { get; set; } = [];

    /// <summary>
    /// Gets a flat dictionary of all nodes in the tree, keyed by their unique identifier.
    /// </summary>
    public Dictionary<Guid, Node> FlatTree { get; private set; } = [];

    /// <summary>
    /// Adds a node to the tree. If the node has a parent, it is added as a child of the parent.
    /// Otherwise, it is added as a root node. Returns true if the node was added successfully; otherwise, false.
    /// </summary>
    /// <param name="node">The node to add to the tree.</param>
    /// <returns>True if the node was added successfully; otherwise, false.</returns>
    public bool Add(Node node) {
      BigInteger bitFlag = 0;
      bool nameAdded = true;

      try {
        bitFlag = GetUnsetBit();
        if (bitFlag == 0) {
          throw new InvalidOperationException("The tree is full. No more nodes can be added.");
        }

        if (!_nodeNames.Add(node.Name)) {
          nameAdded = false;
          throw new InvalidOperationException($"A node with the name '{node.Name}' already exists in the tree. Node names must be unique.");
        }

        if (!FlatTree.TryAdd(node.Id, node)) {
          if (FlatTree[node.Id].Name == "") {
            // If the node was added as a false parent, update it.
            UpdateFalseParent(node);
            return true;
          } else {
            if (nameAdded) {
              _nodeNames.Remove(node.Name);
            }
            return false;
          }
        }

        _bitFlags |= bitFlag;
        node.BitFlag = bitFlag;

        if (node.ParentId != Guid.Empty) {
          if (!FlatTree.TryGetValue(node.ParentId, out Node parent)) {
            parent = AddFalseParent(node);
          }
          parent.AddChild(node);
        } else {
          Roots.Add(node);
        }
        return true;
      } catch (Exception) {
        Roots.Remove(node);
        FlatTree.Remove(node.Id);
        if (nameAdded) {
          _nodeNames.Remove(node.Name);
        }
        if (bitFlag != 0) {
          _bitFlags &= ~bitFlag;
        }

        return false;
      }
    }

    /// <summary>
    /// Removes a node from the tree. If the node has children, they are also removed iteratively.
    /// Returns true if the node and its children were removed successfully; otherwise, false.
    /// </summary>
    /// <param name="nodeToRemove">The node to remove from the tree.</param>
    /// <returns>True if the node was removed successfully; otherwise, false.</returns>
    public bool Remove(Node nodeToRemove) {
      List<Node> removed = [];
      Stack<Node> nodesToRemove = [];
      nodesToRemove.Push(nodeToRemove);

      bool removedRoot = false;
      bool removedChild = false;

      try {
        while (nodesToRemove.TryPeek(out Node current)) {
          if (current.Children.Count > 0) {
            foreach (Node child in current.Children) {
              nodesToRemove.Push(child);
            }
            continue;
          }

          nodesToRemove.Pop();

          if (!current.IsFalseParent) {
            if (current.ParentId == Guid.Empty) {
              if (!Roots.Remove(current)) {
                return false;
              }
              removedRoot = true;
            } else {
              if (current.ParentNode.RemoveChild(current) is null) {
                return false;
              }
              removedChild = true;
            }
          }

          if (!FlatTree.Remove(current.Id)) {
            if (removedRoot) {
              Roots.Add(current);
            }
            if (removedChild) {
              current.ParentNode.AddChild(current);
            }
            return false;
          }

          _bitFlags &= ~current.BitFlag;
          _nodeNames.Remove(current.Name);
        }
        removed.Add(nodeToRemove);
        return true;
      } catch (Exception ex) {
        Console.Error.WriteLine($"Error removing node: {ex}");
        // Attempt to restore any nodes that were removed before the error occurred.
        foreach (Node node in removed) {
          Add(node);
        }

        return false;
      }
    }

    /// <summary>
    /// Cleans the tree by removing any false parents that were never filled in and any orphaned nodes.
    /// </summary>
    public void CleanTree() {
      // Clear out any false parents that were never filled in.
      List<Node> falseParents = [.. FlatTree.Values.Where(n => n.Name == "")];
      foreach (Node falseParent in falseParents) {
        Remove(falseParent);
      }

      // Clear out any orphaned nodes
      List<Node> orphanedNodes = [.. FlatTree.Values.Where(n => n.ParentId != Guid.Empty && !FlatTree.ContainsKey(n.ParentId))];
      foreach (Node orphan in orphanedNodes) {
        Remove(orphan);
      }
    }

    /// <summary>
    /// Removes all nodes from the tree, resetting its state to empty.
    /// </summary>
    public void Clear() {
      Roots.Clear();
      FlatTree.Clear();
      _nodeNames.Clear();
      _bitFlags = 0;
    }

    /// <summary>
    /// Determines whether the tree contains the specified node instance.
    /// </summary>
    /// <param name="node">The node to locate in the tree.</param>
    /// <returns>True if the node exists in the tree; otherwise, false.</returns>
    public bool Contains(Node node) {
      if (!FlatTree.TryGetValue(node.Id, out Node value)) {
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
    /// Determines whether the tree contains a node with the specified name.
    /// </summary>
    /// <param name="name">The name of the node to locate.</param>
    /// <returns>True if a node with the specified name exists in the tree; otherwise, false.</returns>
    public bool Contains(string name) {
      return _nodeNames.Contains(name);
    }

    /// <summary>
    /// Retrieves a node from the tree by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the node to retrieve.</param>
    /// <returns>The node with the specified identifier, or null if not found.</returns>
    public Node GetNodeById(Guid id) {
      if (FlatTree.TryGetValue(id, out Node node)) {
        return node;
      }
      return null;
    }

    /// <summary>
    /// Retrieves a node from the tree by its name.
    /// </summary>
    /// <param name="name">The name of the node to retrieve.</param>
    /// <returns>The node with the specified name, or null if not found.</returns>
    public Node GetNodeByName(string name) {
      return FlatTree.Values.FirstOrDefault(n => n.Name == name);
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

    private void UpdateFalseParent(Node node) {
      // Do not change the bit flag as the correct value was assigned when the false parent
      // was originally added.
      FlatTree[node.Id].ParentId = node.ParentId;
      FlatTree[node.Id].Name = node.Name;
      FlatTree[node.Id].IsFalseParent = false;

      if (node.ParentId == Guid.Empty) {
        if (!Roots.Contains(FlatTree[node.Id])) {
          Roots.Add(FlatTree[node.Id]);
        }
      }
    }

    private Node AddFalseParent(Node childNode) {
      BigInteger falseParentBitFlag = GetUnsetBit();
      if (falseParentBitFlag == 0) {
        throw new InvalidOperationException("The tree is full. Error adding parent node.");
      }
      _bitFlags |= falseParentBitFlag;

      Node falseParent = new("") {
        IsFalseParent = true,
        Id = childNode.ParentId,
        BitFlag = falseParentBitFlag
      };
      FlatTree.Add(falseParent.Id, falseParent);
      return falseParent;
    }
  }
}
