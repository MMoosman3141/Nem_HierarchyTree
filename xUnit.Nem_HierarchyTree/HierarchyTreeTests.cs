using Nem_HierarchyTree;
using System.Numerics;
using System.Text.Json;

namespace xUnit.Nem_HierarchyTree;
public class HierarchyTreeTests {
  [Fact]
  public void Performance() {
    HierarchyTree tree = new() {
      MaxNodes = 5_000
    };
    int num = 1;
    Node node = new($"node{num}");

    while(tree.Add(node)) {
      num++;
      node = new($"node{num}") {
        ParentId = node.Id
      };
    }

    Assert.Equal(tree.MaxNodes, tree.Count);
    Assert.Equal(BigInteger.Pow(2, tree.MaxNodes) - 1, tree.Roots[0].CheckValue);

    tree.Remove(tree.Roots[0]);

    Assert.Equal(0, tree.Count);
  }

  [Fact]
  public void CreateDuplicateNodeName() {
    HierarchyTree tree = new();
    Node parent = new("Parent");
    Node child1 = new("Child") {
      ParentId = parent.Id
    };
    Node child2 = new("Child") {
      ParentId = parent.Id
    };

    Assert.True(tree.Add(parent));
    Assert.True(tree.Add(child1));
    Assert.False(tree.Add(child2));

    Assert.True(tree.FlatTree.ContainsKey(parent.Id));
    Assert.True(tree.FlatTree.ContainsKey(child1.Id));
    Assert.False(tree.FlatTree.ContainsKey(child2.Id));
  }

  [Fact]
  public void CreateDuplicateNodeId() {
    HierarchyTree tree = new();
    Node parent = new("Parent");

    Guid childId = Guid.NewGuid();

    Node child1 = new("Child1") {
      Id = childId,
      ParentId = parent.Id
    };
    Node child2 = new("Child2") {
      Id = childId,
      ParentId = parent.Id
    };

    Assert.True(tree.Add(parent));
    Assert.True(tree.Add(child1));
    Assert.False(tree.Add(child2));

    Assert.True(tree.FlatTree.ContainsKey(parent.Id));
    Assert.True(tree.FlatTree.ContainsKey(child1.Id));
    Assert.NotEqual(child2.Name, tree.FlatTree[child2.Id].Name);
  }

  [Fact]
  public void SerializeDeserializeJson() {
    HierarchyTree originalTree = new();

    Node parent1 = new("Parent 1");
    Node child11 = new("Child 1.1") {
      ParentId = parent1.Id
    };
    Node child12 = new("Child 1.2") {
      ParentId = parent1.Id
    };
    Node child121 = new("Child 1.2.1") {
      ParentId = child12.Id
    };

    // Adding out of order is intentional to test ability to link parents to existing children correctly
    originalTree.Add(child121);
    originalTree.Add(child11);
    originalTree.Add(child12);
    originalTree.Add(parent1);

    Node parent2 = new("Parent 2");
    Node child21 = new("Child 2.1") {
      ParentId = parent2.Id
    };
    Node child22 = new("Child 2.2") {
      ParentId = parent2.Id
    };

    originalTree.Add(parent2);
    originalTree.Add(child21);
    originalTree.Add(child22);

    Node parent3 = new("Parent 3");
    Node child31 = new("Child 3.1") {
      ParentId = parent3.Id
    };
    Node child32 = new("Child 3.2") {
      ParentId = parent3.Id
    };

    originalTree.Add(child31);
    originalTree.Add(parent3);
    originalTree.Add(child32);

    string json = JsonSerializer.Serialize(originalTree);
    HierarchyTree tree = JsonSerializer.Deserialize<HierarchyTree>(json);

    Assert.Equal(3, tree.Roots.Count);
    foreach (Node expectedRoot in originalTree.Roots) {
      Assert.Contains(expectedRoot, tree.Roots);
    }

    Assert.Equal(10, tree.FlatTree.Count);
    foreach (Node expectedNode in originalTree.FlatTree.Values) {
      Assert.Contains(expectedNode, tree.FlatTree.Values);
    }

  }

  [Fact]
  public void Add_NodeWithoutParent_AddsToRootsAndFlatTree() {
    HierarchyTree tree = new();
    Node node = new("Root");

    bool result = tree.Add(node);

    Assert.True(result);
    Assert.Contains(node, tree.Roots);
    Assert.True(tree.FlatTree.ContainsKey(node.Id));
  }

  [Fact]
  public void Add_NodeWithParent_AddsAsChildAndToFlatTree() {
    HierarchyTree tree = new();
    Node parent = new("Parent");
    tree.Add(parent);

    Node child = new("Child") {
      ParentId = parent.Id
    };
    bool result = tree.Add(child);

    Assert.True(result);
    Assert.Contains(child, parent.Children);
    Assert.True(tree.FlatTree.ContainsKey(child.Id));
  }

  [Fact]
  public void Remove_NodeWithoutChildren_RemovesFromRootsAndFlatTree() {
    HierarchyTree tree = new();
    Node node = new("Root");
    tree.Add(node);

    bool result = tree.Remove(node);

    Assert.True(result);
    Assert.DoesNotContain(node, tree.Roots);
    Assert.False(tree.FlatTree.ContainsKey(node.Id));
  }

  [Fact]
  public void Remove_NodeWithChildren_RemovesRecursively() {
    HierarchyTree tree = new();
    Node parent = new("Parent");
    tree.Add(parent);

    Node subParent1 = new("SubParent1") {
      ParentId = parent.Id,
    };
    tree.Add(subParent1);
    Node subParent2 = new("SubParent2") {
      ParentId = parent.Id,
    };
    tree.Add(subParent2);

    Node child1 = new("Child1") {
      ParentId = subParent1.Id,
    };
    tree.Add(child1);
    Node child2 = new("Child2") {
      ParentId = subParent2.Id,
    };
    tree.Add(child2);

    bool result = tree.Remove(parent);

    Assert.True(result);
    Assert.False(tree.FlatTree.ContainsKey(parent.Id));
    Assert.False(tree.FlatTree.ContainsKey(subParent1.Id));
    Assert.False(tree.FlatTree.ContainsKey(subParent2.Id));
    Assert.False(tree.FlatTree.ContainsKey(child1.Id));
    Assert.False(tree.FlatTree.ContainsKey(child2.Id));
  }

  [Fact]
  public void Remove_ChildrenUp() {
    HierarchyTree tree = new();
    Node parent = new("Parent");
    tree.Add(parent);

    Node subParent1 = new("SubParent1") {
      ParentId = parent.Id,
    };
    tree.Add(subParent1);
    Node subParent2 = new("SubParent2") {
      ParentId = parent.Id,
    };
    tree.Add(subParent2);

    Node child1 = new("Child1") {
      ParentId = subParent1.Id,
    };
    tree.Add(child1);
    Node child2 = new("Child2") {
      ParentId = subParent2.Id,
    };
    tree.Add(child2);

    bool result = tree.Remove(child1);
    Assert.True(result);

    result = tree.Remove(child2);
    Assert.True(result);

    result = tree.Remove(subParent1);
    Assert.True(result);

    result = tree.Remove(subParent2);
    Assert.True(result);

    result = tree.Remove(parent);
    Assert.True(result);

    Assert.False(tree.FlatTree.ContainsKey(parent.Id));
    Assert.False(tree.FlatTree.ContainsKey(subParent1.Id));
    Assert.False(tree.FlatTree.ContainsKey(subParent2.Id));
    Assert.False(tree.FlatTree.ContainsKey(child1.Id));
    Assert.False(tree.FlatTree.ContainsKey(child2.Id));
  }

  [Fact]
  public void Remove_ReaddWorks() {
    HierarchyTree tree = new();
    Node parent = new("Parent");
    tree.Add(parent);

    Node subParent1 = new("SubParent1") {
      ParentId = parent.Id,
    };
    tree.Add(subParent1);
    Node subParent2 = new("SubParent2") {
      ParentId = parent.Id,
    };
    tree.Add(subParent2);

    Node child1 = new("Child1") {
      ParentId = subParent1.Id,
    };
    tree.Add(child1);
    Node child2 = new("Child2") {
      ParentId = subParent2.Id,
    };
    tree.Add(child2);

    bool result = tree.Remove(parent);
    Assert.True(result);

    tree.Add(parent);
    tree.Add(subParent1);
    tree.Add(subParent2);
    tree.Add(child1);
    tree.Add(child2);

    result = tree.Remove(parent);

    Assert.True(result);
    Assert.False(tree.FlatTree.ContainsKey(parent.Id));
    Assert.False(tree.FlatTree.ContainsKey(subParent1.Id));
    Assert.False(tree.FlatTree.ContainsKey(subParent2.Id));
    Assert.False(tree.FlatTree.ContainsKey(child1.Id));
    Assert.False(tree.FlatTree.ContainsKey(child2.Id));
  }

  [Fact]
  public void Add_NodeWithNonexistentParent_CreatesFalseParent() {
    HierarchyTree tree = new();
    Node node = new("Child") {
      ParentId = Guid.NewGuid()
    };

    bool result = tree.Add(node);

    Assert.True(result);
    Assert.True(tree.FlatTree.ContainsKey(node.ParentId));
    Assert.True(tree.FlatTree.ContainsKey(node.Id));
    Assert.Contains(node, tree.FlatTree[node.ParentId].Children);
  }

  [Fact]
  public void MaximumSize() {
    HierarchyTree tree = new();

    for (int i = 0; i < tree.MaxNodes; i++) {
      tree.Add(new Node($"Node {i}"));
    }

    Assert.False(tree.Add(new Node($"Node {tree.MaxNodes}")));
  }

  [Fact]
  public void UnableToAddFalseParent() {
    HierarchyTree tree = new();

    for (int i = 0; i < tree.MaxNodes - 1; i++) {
      tree.Add(new Node($"Node {i}"));
    }

    Node child = new("Child") {
      ParentId = Guid.NewGuid()
    };

    Assert.False(tree.Add(child));
  }

  [Fact]
  public void CleanTree_RemovesFalseParentsAndOrphans() {
    HierarchyTree tree = new();

    Node parent = new("Parent");
    Node child1 = new("Child1") {
      ParentId = parent.Id
    };

    Node orphan1 = new("Orphan1") {
      ParentId = Guid.NewGuid()
    };
    Node orphan2 = new("Orphan2") {
      ParentId = Guid.NewGuid()
    };
    Node orphan3 = new("Orphan3") {
      ParentId = Guid.NewGuid()
    };

    tree.Add(parent);
    tree.Add(child1);
    tree.Add(orphan1);
    tree.Add(orphan2);
    tree.Add(orphan3);

    // Precondition: all nodes present
    Assert.Equal(8, tree.Count);
    Assert.True(tree.Contains(parent.Id));
    Assert.True(tree.Contains(child1.Id));
    Assert.True(tree.Contains(orphan1.Id));
    Assert.True(tree.Contains(orphan2.Id));
    Assert.True(tree.Contains(orphan3.Id));
    Assert.True(tree.Contains(orphan1.ParentId));
    Assert.True(tree.Contains(orphan2.ParentId));
    Assert.True(tree.Contains(orphan3.ParentId));

    tree.CleanTree();

    // False parent and orphan should be removed
    Assert.Equal(2, tree.Count);
    Assert.True(tree.Contains(parent.Id));
    Assert.True(tree.Contains(child1.Id));
    Assert.False(tree.Contains(orphan1.Id));
    Assert.False(tree.Contains(orphan2.Id));
    Assert.False(tree.Contains(orphan3.Id));
    Assert.False(tree.Contains(orphan1.ParentId));
    Assert.False(tree.Contains(orphan2.ParentId));
    Assert.False(tree.Contains(orphan3.ParentId));
  }

  [Fact]
  public void CleanTree_DoesNothingIfTreeIsClean() {
    HierarchyTree tree = new();
    Node root = new("Root");
    Node child = new("Child") {
      ParentId = root.Id 
    };
    tree.Add(root);
    tree.Add(child);
    int countBefore = tree.Count;
    tree.CleanTree();
    int countAfter = tree.Count;
    Assert.Equal(countBefore, countAfter);
    Assert.True(tree.FlatTree.ContainsKey(root.Id));
    Assert.True(tree.FlatTree.ContainsKey(child.Id));
  }

  [Fact]
  public void ClearTree() {
    HierarchyTree tree = new();

    Node root1 = new("Root1");
    Node root2 = new("Root2");

    Node parent11 = new("Parent1.1") {
      ParentId = root1.Id
    };
    Node parent21 = new("Parent2.1") {
      ParentId = root2.Id
    };

    Node child111 = new("Child1.1.1") {
      ParentId = parent11.Id
    };
    Node child112 = new("Child1.1.2") {
      ParentId = parent11.Id
    };
    Node child221 = new("Child2.2.1") {
      ParentId = parent21.Id
    };
    Node child222 = new("Child2.2.2") {
      ParentId = parent21.Id
    };

    tree.Add(root1);
    tree.Add(root2);
    tree.Add(parent11);
    tree.Add(parent21);
    tree.Add(child111);
    tree.Add(child112);
    tree.Add(child221);
    tree.Add(child222);

    Assert.Equal(8, tree.Count);
    Assert.True(tree.Contains(root1.Name));
    Assert.True(tree.Contains(root2));
    Assert.True(tree.Contains(parent11.Name));
    Assert.True(tree.Contains(parent21));
    Assert.True(tree.Contains(child111.Name));
    Assert.True(tree.Contains(child112));
    Assert.True(tree.Contains(child221.Name));
    Assert.True(tree.Contains(child222));

    tree.Clear();

    Assert.Equal(0, tree.Count);
    Assert.False(tree.Contains(root1));
    Assert.False(tree.Contains(root2.Name));
    Assert.False(tree.Contains(parent11));
    Assert.False(tree.Contains(parent21.Name));
    Assert.False(tree.Contains(child111));
    Assert.False(tree.Contains(child112.Name));
    Assert.False(tree.Contains(child221));
    Assert.False(tree.Contains(child222.Name));
  }

  [Fact]
  public void GetNode() {
    HierarchyTree tree = new();

    Node root1 = new("Root1");
    Node root2 = new("Root2");

    Node parent11 = new("Parent1.1") {
      ParentId = root1.Id
    };
    Node parent21 = new("Parent2.1") {
      ParentId = root2.Id
    };

    Node child111 = new("Child1.1.1") {
      ParentId = parent11.Id
    };
    Node child112 = new("Child1.1.2") {
      ParentId = parent11.Id
    };
    Node child221 = new("Child2.2.1") {
      ParentId = parent21.Id
    };
    Node child222 = new("Child2.2.2") {
      ParentId = parent21.Id
    };

    tree.Add(root1);
    tree.Add(root2);
    tree.Add(parent11);
    tree.Add(parent21);
    tree.Add(child111);
    tree.Add(child112);
    tree.Add(child221);
    tree.Add(child222);

    Assert.Equal(root1, tree.GetNodeById(root1.Id));
    Assert.Equal(root2, tree.GetNodeByName(root2.Name));
    Assert.Equal(parent11, tree.GetNodeById(parent11.Id));
    Assert.Equal(parent21, tree.GetNodeByName(parent21.Name));
    Assert.Equal(child111, tree.GetNodeById(child111.Id));
    Assert.Equal(child112, tree.GetNodeByName(child112.Name));
    Assert.Equal(child221, tree.GetNodeById(child221.Id));
    Assert.Equal(child222, tree.GetNodeByName(child222.Name));
  }
}